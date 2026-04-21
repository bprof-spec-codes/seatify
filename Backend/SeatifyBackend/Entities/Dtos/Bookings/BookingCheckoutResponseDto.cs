using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Bookings
{
    public class BookingCheckoutResponseDto
    {
        public string BookingId { get; set; }
        public string EventId { get; set; }
        public List<string> Seats { get; set; }
        public decimal TotalPrice { get; set; }
        public string QrCodeBase64 { get; set; }
    }
}
