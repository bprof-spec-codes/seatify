using Data;
using Entities.Dtos.Bookings;
using Entities.Models;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class BookingSessionService : IBookingSessionService
    {
        private readonly AppDbContext _ctx;
        private readonly TimeSpan _sessionTtl = TimeSpan.FromMinutes(10);

        public BookingSessionService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public BookingSessionViewDto Create(BookingSessionCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.EventOccurrenceId))
            {
                throw new ArgumentException("EventOccurrenceId is required.");
            }

            var occurrence = _ctx.EventOccurrences.FirstOrDefault(o => o.Id == dto.EventOccurrenceId);
            if (occurrence == null)
            {
                throw new KeyNotFoundException($"EventOccurrence not found: {dto.EventOccurrenceId}");
            }

            if (!(DateTime.UtcNow > occurrence.BookingOpenAtUtc && DateTime.UtcNow < occurrence.BookingCloseAtUtc))
            {
                throw new ArgumentException("Currently there is no booking period for this event occurence.");
            }

            var now = DateTime.UtcNow;
            var session = new BookingSession
            {
                EventOccurrendeId = dto.EventOccurrenceId,
                Phase = "Selection",
                Status = "Active",
                CreatedAtUtc = now,
                ExpiresAtUtc = now.Add(_sessionTtl)
            };

            _ctx.bookingSessions.Add(session);
            _ctx.SaveChanges();

            return BuildView(session);
        }

        public BookingSessionViewDto? GetById(string bookingSessionId)
        {
            var session = _ctx.bookingSessions.FirstOrDefault(s => s.Id == bookingSessionId);
            if (session == null)
            {
                return null;
            }

            if (TryExpireSession(session))
            {
                _ctx.SaveChanges();
            }

            return BuildView(session);
        }

        public BookingSessionViewDto? HoldSeat(string bookingSessionId, BookingSessionHoldDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.SeatId))
            {
                throw new ArgumentException("SeatId is required.");
            }

            var session = _ctx.bookingSessions.FirstOrDefault(s => s.Id == bookingSessionId);
            if (session == null)
            {
                return null;
            }

            if (TryExpireSession(session))
            {
                _ctx.SaveChanges();
                throw new InvalidOperationException("Booking session has expired.");
            }

            var seat = _ctx.Seats.FirstOrDefault(s => s.Id == dto.SeatId);
            if (seat == null)
            {
                throw new KeyNotFoundException($"Seat not found: {dto.SeatId}");
            }

            if (seat.SeatType != SeatType.Seat && seat.SeatType != SeatType.AccessibleSeat)
            {
                throw new ArgumentException("Selected seat is not bookable.");
            }

            var alreadyBookedSeatIds = _ctx.ReservationSeats
                .Include(rs => rs.Reservation)
                .Where(rs => rs.Reservation.EventOccurrenceId == session.EventOccurrendeId && rs.Reservation.Status == "Confirmed")
                .Select(rs => rs.SeatId)
                .ToHashSet();

            if (alreadyBookedSeatIds.Contains(dto.SeatId))
            {
                throw new InvalidOperationException("Seat is already booked.");
            }

            var hasActiveHold = _ctx.seatHolds
                .Include(h => h.BookingSession)
                .Any(h => h.SeatId == dto.SeatId
                    && h.EventOccurrenceId == session.EventOccurrendeId
                    && h.BookingSessionId != session.Id
                    && h.Status == "Held"
                    && h.BookingSession.Status == "Active"
                    && h.BookingSession.ExpiresAtUtc > DateTime.UtcNow);

            if (hasActiveHold)
            {
                throw new InvalidOperationException("Seat is currently held by another session.");
            }

            var existing = _ctx.seatHolds.FirstOrDefault(h => h.BookingSessionId == session.Id && h.SeatId == dto.SeatId);
            if (existing == null)
            {
                var hold = new SeatHold
                {
                    BookingSessionId = session.Id,
                    EventOccurrenceId = session.EventOccurrendeId,
                    SeatId = dto.SeatId,
                    Phase = session.Phase,
                    Status = "Held",
                    CreatedAtUtc = DateTime.UtcNow
                };

                _ctx.seatHolds.Add(hold);
                _ctx.SaveChanges();
            }

            return BuildView(session);
        }

        public BookingSessionViewDto? ReleaseSeat(string bookingSessionId, string seatId)
        {
            var session = _ctx.bookingSessions.FirstOrDefault(s => s.Id == bookingSessionId);
            if (session == null)
            {
                return null;
            }

            var hold = _ctx.seatHolds.FirstOrDefault(h => h.BookingSessionId == session.Id && h.SeatId == seatId);
            if (hold == null)
            {
                return null;
            }

            _ctx.seatHolds.Remove(hold);
            _ctx.SaveChanges();

            return BuildView(session);
        }

        public BookingSessionViewDto? MoveToCheckout(string bookingSessionId)
        {
            var session = _ctx.bookingSessions.FirstOrDefault(s => s.Id == bookingSessionId);
            if (session == null)
            {
                return null;
            }

            if (TryExpireSession(session))
            {
                _ctx.SaveChanges();
                throw new InvalidOperationException("Booking session has expired.");
            }

            session.Phase = "Checkout";
            _ctx.SaveChanges();

            return BuildView(session);
        }

        private BookingSessionViewDto BuildView(BookingSession session)
        {
            var holds = _ctx.seatHolds
                .Where(h => h.BookingSessionId == session.Id)
                .ToList();

            var seatIds = holds.Select(h => h.SeatId).Distinct().ToList();
            var seats = _ctx.Seats
                .Include(s => s.Sector)
                .Where(s => seatIds.Contains(s.Id))
                .ToList();

            var eventOccurrence = _ctx.EventOccurrences
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == session.EventOccurrendeId);

            var eventId = eventOccurrence?.EventId ?? string.Empty;

            var eventOverrides = _ctx.EventSeatOverrides
                .Include(o => o.Sector)
                .Where(o => o.EventId == eventId && seatIds.Contains(o.SeatId))
                .ToList();

            var occurrenceOverrides = _ctx.OccurrenceSeatOverrides
                .Include(o => o.Sector)
                .Where(o => o.OccurrenceId == session.EventOccurrendeId && seatIds.Contains(o.SeatId))
                .ToList();

            var seatMap = seats.ToDictionary(s => s.Id);
            var eventOverrideMap = eventOverrides.ToDictionary(o => o.SeatId);
            var occurrenceOverrideMap = occurrenceOverrides.ToDictionary(o => o.SeatId);

            var holdDtos = new List<SeatHoldViewDto>();
            foreach (var hold in holds)
            {
                if (!seatMap.TryGetValue(hold.SeatId, out var seat))
                {
                    continue;
                }

                eventOverrideMap.TryGetValue(hold.SeatId, out var eventOverride);
                occurrenceOverrideMap.TryGetValue(hold.SeatId, out var occurrenceOverride);

                var basePrice = ResolveFinalPrice(seat, eventOverride, occurrenceOverride);
                var (rowLabel, seatLabel) = ResolveSeatLabels(seat);

                holdDtos.Add(new SeatHoldViewDto
                {
                    Id = hold.Id,
                    SeatId = hold.SeatId,
                    RowLabel = rowLabel,
                    SeatLabel = seatLabel,
                    BasePrice = basePrice
                });
            }

            return new BookingSessionViewDto
            {
                Id = session.Id,
                EventOccurrenceId = session.EventOccurrendeId,
                Phase = session.Phase,
                Status = session.Status,
                CreatedAtUtc = session.CreatedAtUtc,
                ExpiresAtUtc = session.ExpiresAtUtc,
                Holds = holdDtos
            };
        }

        private static (string rowLabel, string seatLabel) ResolveSeatLabels(Seat seat)
        {
            if (!string.IsNullOrWhiteSpace(seat.SeatLabel))
            {
                var label = seat.SeatLabel.Trim();
                var dashIndex = label.IndexOf('-');
                if (dashIndex > 0 && dashIndex < label.Length - 1)
                {
                    var row = label.Substring(0, dashIndex).Trim();
                    var seatLabel = label[(dashIndex + 1)..].Trim();
                    if (!string.IsNullOrWhiteSpace(row) && !string.IsNullOrWhiteSpace(seatLabel))
                    {
                        return (row, seatLabel);
                    }
                }

                var digitIndex = FirstDigitIndex(label);
                if (digitIndex > 0)
                {
                    var row = label.Substring(0, digitIndex).Trim();
                    var seatLabel = label[digitIndex..].Trim();
                    if (!string.IsNullOrWhiteSpace(row) && !string.IsNullOrWhiteSpace(seatLabel))
                    {
                        return (row, seatLabel);
                    }
                }

                return (IntToLetters(seat.Row), label);
            }

            return (IntToLetters(seat.Row), seat.Column.ToString());
        }

        private static int FirstDigitIndex(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsDigit(value[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private static string IntToLetters(int value)
        {
            if (value <= 0) return "-";

            string result = string.Empty;
            while (--value >= 0)
            {
                result = (char)('A' + value % 26) + result;
                value /= 26;
            }

            return string.IsNullOrEmpty(result) ? "-" : result;
        }

        private static decimal ResolveFinalPrice(Seat seat, EventSeatOverride? eventOverride, OccurrenceSeatOverride? occurrenceOverride)
        {
            if (occurrenceOverride?.PriceOverride != null)
            {
                return occurrenceOverride.PriceOverride.Value;
            }

            if (eventOverride?.PriceOverride != null)
            {
                return eventOverride.PriceOverride.Value;
            }

            if (seat.PriceOverride != null)
            {
                return seat.PriceOverride.Value;
            }

            if (occurrenceOverride?.Sector?.BasePrice != null)
            {
                return occurrenceOverride.Sector.BasePrice;
            }

            if (eventOverride?.Sector?.BasePrice != null)
            {
                return eventOverride.Sector.BasePrice;
            }

            if (seat.Sector?.BasePrice != null)
            {
                return seat.Sector.BasePrice;
            }

            return 0m;
        }

        private bool TryExpireSession(BookingSession session)
        {
            if (session.Status != "Active")
            {
                return false;
            }

            if (session.ExpiresAtUtc > DateTime.UtcNow)
            {
                return false;
            }

            session.Status = "Expired";
            session.Phase = "Expired";

            var holds = _ctx.seatHolds.Where(h => h.BookingSessionId == session.Id).ToList();
            if (holds.Count > 0)
            {
                _ctx.seatHolds.RemoveRange(holds);
            }

            return true;
        }
    }
}
