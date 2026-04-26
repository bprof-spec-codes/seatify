using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public class EmailTicketItem
    {
        public string SeatLabel { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string QrCodeBase64 { get; set; } = string.Empty;
    }

    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(
            string toEmail,
            string customerName,
            string eventName,
            DateTime eventTime,
            IEnumerable<EmailTicketItem> tickets,
            decimal totalPrice,
            string currency);
    }
}
