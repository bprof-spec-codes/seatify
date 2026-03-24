using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.EventOccurrence
{
    public class EventOccurrenceViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public string VenueId { get; set; } = string.Empty;
        public string AuditoriumId { get; set; } = string.Empty;

        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public DateTime BookingOpenAtUtc { get; set; }
        public DateTime BookingCloseAtUtc { get; set; }
        public string Status { get; set; } = string.Empty;

        // TODO: change to EventDto
        public EventViewDto Event { get; set; } = null!;

        // TODO: change to VenueDto
        public VenueViewDto Venue { get; set; } = null!;

        // TODO: change to AuditoriumDto
        public AuditoriumViewDto Auditorium { get; set; } = null!;
    }

    public class EventViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class VenueViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class AuditoriumViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
