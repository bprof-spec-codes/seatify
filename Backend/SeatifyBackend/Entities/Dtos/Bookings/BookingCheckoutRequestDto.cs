using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Bookings
{
    public class BookingCheckoutRequestDto
    {
        public string EventOccurrenceId { get; set; }
        public List<string> SeatIds { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string BookingSessionId { get; set; }
    }
}
