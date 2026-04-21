using Data;
using Entities.Dtos.EventOccurrence;
using Entities.Dtos.Reservation;
using Entities.Models;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public class EventOccurrenceService : IEventOccurrenceService
    {
        private AppDbContext _appDbContext;

        public EventOccurrenceService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public bool Create(EventOccurrenceCreateDto createDto)
        {
            EventOccurrence eventOccurrence = new EventOccurrence()
            {
                EventId = createDto.EventId,
                VenueId = createDto.VenueId,
                AuditoriumId = createDto.AuditoriumId,
                StartsAtUtc = createDto.StartsAtUtc,
                EndsAtUtc = createDto.EndsAtUtc,
                BookingOpenAtUtc = createDto.BookingOpenAtUtc,
                BookingCloseAtUtc = createDto.BookingCloseAtUtc,
                DoorsOpenAtUtc = createDto.DoorsOpenAtUtc,
                Status = createDto.Status
            };

            _appDbContext.EventOccurrences.Add(eventOccurrence);
            var saved = _appDbContext.SaveChanges();

            return saved > 0;
        }

        public List<EventOccurrenceViewDto> GetByEventId(string eventId)
        {
            return _appDbContext.EventOccurrences
                .Include(e => e.Event)
                .Include(e => e.Venue)
                .Include(e => e.Auditorium)
                .Where(e => e.EventId == eventId)
                .Select(e => MapToViewDto(e))
                .ToList();
        }

        public EventOccurrenceViewDto? GetById(string id)
        {
            var occurrence = _appDbContext.EventOccurrences
                .Include(e => e.Event)
                .Include(e => e.Venue)
                .Include(e => e.Auditorium)
                .FirstOrDefault(e => e.Id == id);

            if (occurrence == null) return null;

            return MapToViewDto(occurrence);
        }

        private EventOccurrenceViewDto MapToViewDto(EventOccurrence occurrence)
        {
            return new EventOccurrenceViewDto
            {
                Id = occurrence.Id,
                EventId = occurrence.EventId,
                VenueId = occurrence.VenueId,
                AuditoriumId = occurrence.AuditoriumId,
                StartsAtUtc = occurrence.StartsAtUtc,
                EndsAtUtc = occurrence.EndsAtUtc,
                BookingOpenAtUtc = occurrence.BookingOpenAtUtc,
                BookingCloseAtUtc = occurrence.BookingCloseAtUtc,
                DoorsOpenAtUtc = occurrence.DoorsOpenAtUtc,
                Status = occurrence.Status,

                Event = occurrence.Event != null ? new EventOccurrenceEventDto
                {
                    Id = occurrence.Event.Id,
                    Name = occurrence.Event.Name,
                    Description = occurrence.Event.Description
                } : null!,

                Venue = occurrence.Venue != null ? new EventOccurrenceVenueDto
                {
                    Id = occurrence.Venue.Id,
                    Name = occurrence.Venue.Name
                } : null!,

                Auditorium = occurrence.Auditorium != null ? new EventOccurrenceAuditoriumDto
                {
                    Id = occurrence.Auditorium.Id,
                    Name = occurrence.Auditorium.Name
                } : null!
            };
        }

        public bool Update(string id, EventOccurrenceCreateDto updateDto)
        {
            var occurrence = _appDbContext.EventOccurrences.FirstOrDefault(e => e.Id == id);
            if (occurrence == null) return false;

            occurrence.EventId = updateDto.EventId;
            occurrence.VenueId = updateDto.VenueId;
            occurrence.AuditoriumId = updateDto.AuditoriumId;
            occurrence.StartsAtUtc = updateDto.StartsAtUtc;
            occurrence.EndsAtUtc = updateDto.EndsAtUtc;
            occurrence.BookingOpenAtUtc = updateDto.BookingOpenAtUtc;
            occurrence.BookingCloseAtUtc = updateDto.BookingCloseAtUtc;
            occurrence.DoorsOpenAtUtc = updateDto.DoorsOpenAtUtc;
            occurrence.Status = updateDto.Status;
            occurrence.UpdatedAtUtc = DateTime.UtcNow;

            return _appDbContext.SaveChanges() > 0;
        }

        public bool Delete(string id)
        {
            var occurrence = _appDbContext.EventOccurrences.FirstOrDefault(e => e.Id == id);
            if (occurrence == null) return false;

            _appDbContext.EventOccurrences.Remove(occurrence);
            return _appDbContext.SaveChanges() > 0;
        }

        public List<ReservationViewDto> GetReservations(string id)
        {
            return _appDbContext.Reservations
                .Include(r => r.ReservationSeats)
                .Where(r => r.EventOccurrenceId == id)
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
                })
                .ToList();
        }
    }
}
