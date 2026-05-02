using System.Collections.Generic;

namespace Logic.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateTicketPdf(
            string eventName,
            string venueName,
            string auditoriumName,
            DateTime eventTime,
            IEnumerable<PdfTicketItem> tickets,
            string currency);
    }

    public class PdfTicketItem
    {
        public string SeatLabel { get; set; } = string.Empty;
        public string Row { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string QrCodeBase64 { get; set; } = string.Empty;
        public string ManualCode { get; set; } = string.Empty;
    }
}
