using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class EventOccurrence
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string EventId { get; set; } = string.Empty;
        public string VenueId { get; set; } = string.Empty;
        public string AuditoriumId { get; set; } = string.Empty;

        public DateTime StartsAtUtc { get; set; }
        public DateTime EndsAtUtc { get; set; }
        public string Status { get; set; } = "Published";

        public DateTime BookingOpenAtUtc { get; set; }
        public DateTime BookingCloseAtUtc { get; set; }

        public DateTime? DoorsOpenAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual Event Event { get; set; } = null!;
        public virtual Venue Venue { get; set; } = null!;
        public virtual Auditorium Auditorium { get; set; } = null!;
        public virtual List<Reservation> Reservations { get; set; } = new();

        // TODO: BookingSession

    }
}
