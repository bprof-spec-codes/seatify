using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Reservation
{
    public class ReservationCreateDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string EventOccurenceId { get; set; } = string.Empty;

        public List<string> SeatIds { get; set; } = new();

        // TODO: later BookingSessionId
    }
}
