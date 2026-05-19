using Logic.Interfaces;
using Logic.Services;
using Moq;
using NUnit.Framework;
using Resend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class EmailServiceTests
    {
        private Mock<IResend> _mockResend;
        private EmailService _emailService;

        [SetUp]
        public void Setup()
        {
            _mockResend = new Mock<IResend>();
            _emailService = new EmailService(_mockResend.Object);
        }

        [Test]
        public async Task SendBookingConfirmationAsync_WithValidData_SendsEmailCorrectly()
        {
            // Arrange
            var toEmail = "customer@example.com";
            var customerName = "Test Customer";
            var eventName = "Test Event Concert";
            var eventTime = new DateTime(2025, 10, 10, 20, 0, 0);
            var tickets = new List<EmailTicketItem>
            {
                new EmailTicketItem { SeatLabel = "Row A Seat 1", Price = 50 },
                new EmailTicketItem { SeatLabel = "Row A Seat 2", Price = 50 }
            };
            var totalPrice = 100m;
            var currency = "USD";
            var pdfAttachment = new byte[] { 1, 2, 3 }; // Mock PDF bytes

            // Act
            await _emailService.SendBookingConfirmationAsync(
                toEmail,
                customerName,
                eventName,
                eventTime,
                tickets,
                totalPrice,
                currency,
                pdfAttachment
            );

            // Assert
            _mockResend.Verify(r => r.EmailSendAsync(It.Is<EmailMessage>(msg => 
                msg.To.Contains(toEmail) &&
                msg.Subject == $"Booking Confirmation: {eventName}" &&
                msg.HtmlBody.Contains(customerName) &&
                msg.HtmlBody.Contains(eventName) &&
                msg.HtmlBody.Contains("Row A Seat 1") &&
                msg.HtmlBody.Contains("100") && // Total price
                msg.Attachments != null && msg.Attachments.Count == 1 && msg.Attachments[0].Filename == "Seatify_Tickets.pdf"
            ), default), Times.Once);
        }

        [Test]
        public async Task SendBookingConfirmationAsync_WithoutAttachment_SendsEmailCorrectly()
        {
            // Arrange
            var tickets = new List<EmailTicketItem>
            {
                new EmailTicketItem { SeatLabel = "VIP 1", Price = 100 }
            };

            // Act
            await _emailService.SendBookingConfirmationAsync(
                "test@test.com",
                "Name",
                "Event",
                DateTime.Now,
                tickets,
                100m,
                "EUR",
                null // No attachment
            );

            // Assert
            _mockResend.Verify(r => r.EmailSendAsync(It.Is<EmailMessage>(msg => 
                msg.To.Contains("test@test.com") &&
                (msg.Attachments == null || msg.Attachments.Count == 0)
            ), default), Times.Once);
        }
    }
}
