using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet("send-test")]
        public async Task<IActionResult> SendTestEmail([FromQuery] string toEmail)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return BadRequest(new { message = "Please provide an email address." });

            try
            {
                // A transparent 1x1 pixel base64 for mockup QR Code purposes
                var dummyQrBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";
                
                var tickets = new List<EmailTicketItem>
                {
                    new EmailTicketItem { SeatLabel = "A-12", Price = 21250m, QrCodeBase64 = dummyQrBase64 },
                    new EmailTicketItem { SeatLabel = "A-13", Price = 21250m, QrCodeBase64 = dummyQrBase64 }
                };

                await _emailService.SendBookingConfirmationAsync(
                    toEmail: toEmail,
                    customerName: "Jane Doe",
                    eventName: "Coldplay - Music of the Spheres",
                    eventTime: DateTime.UtcNow.AddDays(7),
                    tickets: tickets,
                    totalPrice: 42500m
                );

                return Ok(new { message = $"Test email sent successfully to {toEmail} from confirmation@seatify.hu!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error sending email.", error = ex.Message });
            }
        }
    }
}
