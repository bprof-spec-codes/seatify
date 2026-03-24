using Data;
using Entities.Dtos.EventOccurrence;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class EventOccurrenceService
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
                Status = createDto.Status
            };

            _appDbContext.EventOccurrences.Add(eventOccurrence);
            var saved = _appDbContext.SaveChanges();

            return saved > 0;
        }

        public EventOccurrenceViewDto? GetById(string id)
        {
            var occurrence = _appDbContext.EventOccurrences
                .Include(e => e.Event)
                .Include(e => e.Venue)
                .Include(e => e.Auditorium)
                .FirstOrDefault(e => e.Id == id);

            if (occurrence == null) return null;

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
                Status = occurrence.Status,
                Event = occurrence.Event,
                Venue = occurrence.Venue,
                Auditorium = occurrence.Auditorium
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
            occurrence.Status = updateDto.Status;
            occurrence.UpdatedAtUtc = System.DateTime.UtcNow;

            return _appDbContext.SaveChanges() > 0;
        }

        public bool Delete(string id)
        {
            var occurrence = _appDbContext.EventOccurrences.FirstOrDefault(e => e.Id == id);
            if (occurrence == null) return false;

            _appDbContext.EventOccurrences.Remove(occurrence);
            return _appDbContext.SaveChanges() > 0;
        }

        // temporary solution for Reservations
        public object GetReservations(string id)
        {
            // TODO: Return reservation...
            return new { Message = "Reservations feature is under construction." };
        }
    }
}
