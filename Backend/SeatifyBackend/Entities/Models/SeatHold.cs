using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class SeatHold
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string BookingSessionId { get; set; }

        public string EventOccurrenceId { get; set; }

        public string SeatId { get; set; }

        [Required]
        public string Phase { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public virtual BookingSession BookingSession { get; set; }
        public virtual EventOccurrence EventOccurrence { get; set; }
        public virtual Seat Seat { get; set; }
    }
}
