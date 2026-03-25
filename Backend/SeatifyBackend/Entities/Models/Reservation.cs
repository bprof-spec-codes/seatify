using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Reservation
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BookingSessionId { get; set; } = string.Empty;
        public string EventOccurrenceId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string Status { get; set; } = "Confirmed";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual EventOccurrence EventOccurrence { get; set; } = null!;
        public virtual List<ReservationSeat> ReservationSeats { get; set; } = new();

        //public virtual List<BookingSession> BookingSession { get; set; } = new();


    }
}
