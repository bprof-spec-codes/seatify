using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic.Interfaces
{
    public class EmailTicketItem
    {
        public string SeatLabel { get; set; }
        public decimal Price { get; set; }
        public string QrCodeBase64 { get; set; }
    }

    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(
            string toEmail,
            string customerName,
            string eventName,
            DateTime eventTime,
            IEnumerable<EmailTicketItem> tickets,
            decimal totalPrice);
    }
}
