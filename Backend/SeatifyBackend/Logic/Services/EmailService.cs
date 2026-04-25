using Logic.Interfaces;
using Resend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task SendBookingConfirmationAsync(
            string toEmail,
            string customerName,
            string eventName,
            DateTime eventTime,
            IEnumerable<EmailTicketItem> tickets,
            decimal totalPrice,
            string currency)
        {
            var message = new EmailMessage();
            message.From = "Seatify <confirmation@seatify.hu>";
            message.To.Add(toEmail);
            message.Subject = $"Booking Confirmation: {eventName}";

            var eventTimeFormatted = eventTime.ToString("f");
            
            var ticketRowsHtml = new StringBuilder();
            var qrSectionsHtml = new StringBuilder();

            foreach (var ticket in tickets)
            {
                ticketRowsHtml.Append($@"
                <tr>
                    <td style='padding: 12px; border-bottom: 1px solid #e2e8f0; text-align: left;'>{ticket.SeatLabel}</td>
                    <td style='padding: 12px; border-bottom: 1px solid #e2e8f0; text-align: right;'>{ticket.Price:N0} {currency}</td>
                </tr>");

                qrSectionsHtml.Append($@"
                <div class='qr-section'>
                    <p style='margin-top:0; color: #4a5568;'><strong>Check-In QR Code: {ticket.SeatLabel}</strong></p>
                    <img src='data:image/png;base64,{ticket.QrCodeBase64}' alt='QR Code for {ticket.SeatLabel}' />
                    <p style='margin-bottom:0; font-size: 14px; color: #718096;'>Please present this code at the entrance for this seat.</p>
                </div>");
            }

            message.HtmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f4f7f6;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        .header {{
            background-color: #1a202c;
            color: #ffffff;
            text-align: center;
            padding: 24px;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            letter-spacing: 1px;
        }}
        .content {{
            padding: 32px;
            color: #2d3748;
        }}
        .content h2 {{
            color: #2b6cb0;
            margin-top: 0;
        }}
        .details-table {{
            width: 100%;
            border-collapse: collapse;
            margin: 24px 0;
        }}
        .summary-header {{
            color: #718096;
            font-weight: 600;
        }}
        .summary-header td {{
           padding: 12px;
           border-bottom: 2px solid #cbd5e0;
        }}
        .total-row td {{
            font-weight: bold;
            font-size: 18px;
            padding-top: 16px;
        }}
        .qr-section {{
            text-align: center;
            margin-top: 24px;
            padding: 24px;
            background-color: #f7fafc;
            border-radius: 8px;
            border: 1px dashed #cbd5e0;
        }}
        .qr-section img {{
            width: 200px;
            height: 200px;
            margin-bottom: 16px;
        }}
        .footer {{
            background-color: #edf2f7;
            text-align: center;
            padding: 16px;
            color: #a0aec0;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Ticket Confirmation</h1>
        </div>
        <div class='content'>
            <h2>Hello, {customerName}!</h2>
            <p>Thank you for your purchase. Your booking for <strong>{eventName}</strong> on {eventTimeFormatted} has been confirmed successfully.</p>
            
            <table class='details-table'>
                <tr class='summary-header'>
                    <td>Seat</td>
                    <td style='text-align: right;'>Price</td>
                </tr>
                {ticketRowsHtml}
                <tr class='total-row'>
                    <td style='text-align: right;'>Total Price:</td>
                    <td style='text-align: right;'>{totalPrice:N0} {currency}</td>
                </tr>
            </table>

            <h3>Your Tickets</h3>
            {qrSectionsHtml}

        </div>
        <div class='footer'>
            &copy; {DateTime.UtcNow.Year} Seatify. All rights reserved.
        </div>
    </div>
</body>
</html>";

            await _resend.EmailSendAsync(message);
        }
    }
}
