using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Bookings
{
    public class BookingCheckoutResponseDto
    {
        public string BookingId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public List<TicketDto> Tickets { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "EUR";
        public string QrCodeBase64 { get; set; } = string.Empty;
        public string PdfBase64 { get; set; } = string.Empty;
    }

    public class TicketDto
    {
        public string TicketId { get; set; } = string.Empty;
        public string SeatId { get; set; } = string.Empty;
        public string SeatLabel { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
