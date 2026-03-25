using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Reservation
{
    public class ReservationViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public List<ReservationSeatViewDto> ReservedSeats { get; set; } = new();
    }

    public class ReservationSeatViewDto
    {
        public string SeatId { get; set; } = string.Empty;
        public decimal FinalPrice { get; set; }
    }
}
