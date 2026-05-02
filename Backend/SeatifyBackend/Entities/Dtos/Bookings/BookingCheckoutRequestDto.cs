using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Bookings
{
    public class BookingCheckoutRequestDto
    {
        public string EventOccurrenceId { get; set; } = string.Empty;
        public List<string> SeatIds { get; set; } = new();
        public string? CustomerName { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? BookingSessionId { get; set; }
    }
}
