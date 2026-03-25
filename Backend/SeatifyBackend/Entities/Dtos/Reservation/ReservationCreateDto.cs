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
        public string EventOccurrenceId { get; set; } = string.Empty;

        public List<ReservationSeatDto> Seats { get; set; } = new();

        // TODO: later BookingSessionId
    }
    public class ReservationSeatDto
    {
        public string SeatId { get; set; } = string.Empty;
    }
}
