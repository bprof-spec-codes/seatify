using Entities.Dtos.BookingSession;
using Entities.Models;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly BookingService _bookingService;
    private readonly QrService _qrService;

    public BookingsController(BookingService bookingService, QrService qrSrevice)
    {
        _bookingService = bookingService;
        _qrService = qrSrevice;
    }
    
    [HttpPost("checkout")]
    public async Task<IActionResult> CreateBookingsSession([FromBody] BookingSessionCreateDto bookingSessionCreateDto)
    {
        try
        {
            BookingSession newBookingSession = await _bookingService.CreateBookingSessionAsync(bookingSessionCreateDto);
            
            string qrContent = newBookingSession.Id;
            var qrBytes = _qrService.GenerateQrCode(qrContent);

            return File(qrBytes, "image/png");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
