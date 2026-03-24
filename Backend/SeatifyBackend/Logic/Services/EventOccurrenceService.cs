using Data;
using Entities.Dtos.EventOccurrence;
using Entities.Models;
using Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class EventOccurrenceService: IEventOccurrenceService
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

            // temp. solution
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

                // Manual mapping to DTOs to avoid CS0029
                Event = new EventViewDto
                {
                    Id = occurrence.Event.Id,
                    Name = occurrence.Event.Name,
                    Description = occurrence.Event.Description
                },
                Venue = new VenueViewDto
                {
                    Id = occurrence.Venue.Id,
                    Name = occurrence.Venue.Name
                },
                Auditorium = new AuditoriumViewDto
                {
                    Id = occurrence.Auditorium.Id,
                    Name = occurrence.Auditorium.Name
                }
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

        // tem. solution for reservation
        public object GetReservations(string id)
        {
            // TODO: return reservation list...
            return new { Message = "Reservations feature is under construction." };
        }
    }
}
