using Data;
using Entities.Dtos.Bookings;
using Entities.Dtos.Exceptions;
using Entities.Dtos.Reservation;
using Entities.Models;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class ReservationService: IReservationService
    {
        private readonly AppDbContext _context;
        private readonly QrService _qrService;
        private readonly IEmailService _emailService;

        public ReservationService(AppDbContext context, QrService qrService, IEmailService emailService)
        {
            _context = context;
            _qrService = qrService;
            _emailService = emailService;
        }

        public bool CreateReservation(string eventOccurrenceId, ReservationCreateDto dto)
        {
            var reservation = new Reservation
            {
                EventOccurrenceId = eventOccurrenceId,
                CustomerName = dto.CustomerName,
                CustomerEmail = dto.CustomerEmail,
                CustomerPhone = dto.CustomerPhone,
                Status = "Confirmed",
                CreatedAtUtc = DateTime.UtcNow,
                ReservationSeats = new List<ReservationSeat>()
            };

            foreach (var seat in dto.Seats)
            {
                reservation.ReservationSeats.Add(new ReservationSeat
                {
                    SeatId = seat.SeatId,
                    FinalPrice = 5000 // mock data for testing
                });
            }

            _context.Reservations.Add(reservation);
            return _context.SaveChanges() > 0;
        }

        public ReservationViewDto? GetById(string id)
        {
            var res = _context.Reservations
                .Include(r => r.ReservationSeats)
                .FirstOrDefault(r => r.Id == id);

            if (res == null) return null;

            return new ReservationViewDto
            {
                Id = res.Id,
                CustomerName = res.CustomerName,
                CustomerEmail = res.CustomerEmail,
                Status = res.Status,
                CreatedAtUtc = res.CreatedAtUtc,
                ReservedSeats = res.ReservationSeats.Select(rs => new ReservationSeatViewDto
                {
                    SeatId = rs.SeatId,
                    FinalPrice = rs.FinalPrice
                }).ToList()
            };
        }

        public List<ReservationViewDto> GetByOccurrenceId(string eventOccurrenceId)
        {
            return _context.Reservations
                .Include(r => r.ReservationSeats)
                .Where(r => r.EventOccurrenceId == eventOccurrenceId)
                .Select(res => new ReservationViewDto
                {
                    Id = res.Id,
                    CustomerName = res.CustomerName,
                    CustomerEmail = res.CustomerEmail,
                    Status = res.Status,
                    CreatedAtUtc = res.CreatedAtUtc,
                    ReservedSeats = res.ReservationSeats.Select(rs => new ReservationSeatViewDto
                    {
                        SeatId = rs.SeatId,
                        FinalPrice = rs.FinalPrice
                    }).ToList()
                }).ToList();
        }

        public bool UpdateReservation(string id, ReservationUpdateDto dto)
        {
            var res = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (res == null) return false;

            res.CustomerName = dto.CustomerName;
            res.CustomerEmail = dto.CustomerEmail;
            res.CustomerPhone = dto.CustomerPhone;
            res.Status = dto.Status;

            return _context.SaveChanges() > 0;
        }

        public bool DeleteReservation(string id)
        {
            var res = _context.Reservations.FirstOrDefault(r => r.Id == id);
            if (res == null) return false;

            _context.Reservations.Remove(res);
            return _context.SaveChanges() > 0;
        }

        public async Task<BookingCheckoutResponseDto> CheckoutReservation(BookingCheckoutRequestDto request)
        {
            if(!string.IsNullOrEmpty(request.BookingSessionId))
            {
                BookingSession bookingSession = _context.bookingSessions.Find(request.BookingSessionId);
                if(bookingSession == null)
                {
                    throw new BookingSessionNotFoundException($"BookingSession could not be found with this id: {request.BookingSessionId}");
                }
            }

            EventOccurrence eventOccurrence = _context.EventOccurrences
                .Include(eo => eo.Event)
                    .ThenInclude(e => e.Appearance)
                .Include(eo => eo.Auditorium)
                .FirstOrDefault(eo => eo.Id == request.EventOccurrenceId);

            if (eventOccurrence == null)
            {
                throw new EventOccurrenceNotFoundException($"EventOccurrence could not be found with this id: {request.EventOccurrenceId}");
            }

            // Check if any requested seats are already booked for this occurrence
            var alreadyBookedSeats = _context.ReservationSeats
                .Where(rs => rs.Reservation.EventOccurrenceId == request.EventOccurrenceId && rs.Reservation.Status == "Confirmed" && request.SeatIds.Contains(rs.SeatId))
                .Select(rs => rs.SeatId)
                .ToList();

            if (alreadyBookedSeats.Any())
            {
                throw new ArgumentException($"The following seats are already booked: {string.Join(", ", alreadyBookedSeats)}");
            }

            List<ReservationSeat> reservationSeats = new List<ReservationSeat>();
            
            foreach (var seatId in request.SeatIds)
            {
                var finalPrice = calculateFinalSeatPrice(eventOccurrence.Event.Id, eventOccurrence.Id, seatId);
                
                var reservationSeat = new ReservationSeat
                {
                    SeatId = seatId,
                    FinalPrice = finalPrice
                };
                reservationSeats.Add(reservationSeat);
            }

            Reservation reservation = new Reservation();
            reservation.BookingSessionId = request.BookingSessionId ?? string.Empty;
            reservation.EventOccurrenceId = request.EventOccurrenceId;
            reservation.CustomerEmail = request.CustomerEmail;
            reservation.CustomerName = request.CustomerName;
            reservation.CustomerPhone = request.CustomerPhone;
            reservation.ReservationSeats = reservationSeats;
            reservation.Status = "Confirmed";
            reservation.CreatedAtUtc = DateTime.UtcNow;
            
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            // Prepare DTO and Email Tickets
            var ticketDtos = new List<TicketDto>();
            var emailTickets = new List<EmailTicketItem>();

            foreach (var rs in reservationSeats)
            {
                var seatDetails = _context.Seats.FirstOrDefault(s => s.Id == rs.SeatId);
                string seatLabel = seatDetails?.SeatLabel ?? rs.SeatId;
                
                var qrCode = _qrService.GenerateTicketQrCode(rs.Id);

                ticketDtos.Add(new TicketDto
                {
                    SeatId = rs.SeatId,
                    SeatLabel = seatLabel,
                    QrCodeBase64 = qrCode,
                    Price = rs.FinalPrice
                });

                emailTickets.Add(new EmailTicketItem
                {
                    SeatLabel = seatLabel,
                    Price = rs.FinalPrice,
                    QrCodeBase64 = qrCode
                });
            }

            var totalPrice = reservationSeats.Sum(rs => rs.FinalPrice);
            var currency = eventOccurrence.CurrencyOverride 
                           ?? eventOccurrence.Event.Appearance?.Currency 
                           ?? eventOccurrence.Auditorium?.Currency 
                           ?? "HUF";

            // Send confirmation email
            try 
            {
                await _emailService.SendBookingConfirmationAsync(
                    reservation.CustomerEmail,
                    reservation.CustomerName ?? "Customer",
                    eventOccurrence.Event.Name,
                    eventOccurrence.StartsAtUtc,
                    emailTickets,
                    totalPrice,
                    currency
                );
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }

            BookingCheckoutResponseDto responseDto = new BookingCheckoutResponseDto();
            responseDto.BookingId = reservation.Id;
            responseDto.EventId = reservation.EventOccurrenceId;
            responseDto.Tickets = ticketDtos;
            responseDto.TotalPrice = totalPrice;
            responseDto.Currency = currency;
            responseDto.QrCodeBase64 = _qrService.GenerateReservationQrCode(reservation.Id);

            return responseDto;
        }

        private decimal calculateFinalSeatPrice(string eventId, string eventOccurrenceId, string seatId)
        {
            var occurrenceOverride = _context.OccurrenceSeatOverrides
                .FirstOrDefault(os => os.SeatId == seatId && os.OccurrenceId == eventOccurrenceId);
            
            if (occurrenceOverride?.PriceOverride != null)
                return occurrenceOverride.PriceOverride.Value;

            var eventSeatOverride = _context.EventSeatOverrides
                .FirstOrDefault(es => es.SeatId == seatId && es.EventId == eventId);

            if (eventSeatOverride?.PriceOverride != null)
                return eventSeatOverride.PriceOverride.Value;

            var seat = _context.Seats.Include(s => s.Sector).FirstOrDefault(s => s.Id == seatId);
            if (seat == null) return 0;

            if (seat.PriceOverride != null)
                return seat.PriceOverride.Value;

            if (seat.Sector != null)
                return seat.Sector.BasePrice;

            return 0;
        }
    }
}
