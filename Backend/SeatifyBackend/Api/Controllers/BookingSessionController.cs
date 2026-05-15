using Entities.Dtos.Bookings;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/public/booking-sessions")]
    public class BookingSessionController : ControllerBase
    {
        private readonly IBookingSessionService _service;

        public BookingSessionController(IBookingSessionService service)
        {
            _service = service;
        }

        // POST /api/public/booking-sessions
        [HttpPost]
        public IActionResult Create([FromBody] BookingSessionCreateDto dto)
        {
            try
            {
                var session = _service.Create(dto);
                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/public/booking-sessions/{bookingSessionId}
        [HttpGet("{bookingSessionId}")]
        public IActionResult GetById(string bookingSessionId)
        {
            var session = _service.GetById(bookingSessionId);
            if (session == null)
            {
                return NotFound(new { message = "Booking session not found." });
            }

            return Ok(session);
        }

        // POST /api/public/booking-sessions/{bookingSessionId}/holds
        [HttpPost("{bookingSessionId}/holds")]
        public IActionResult HoldSeat(string bookingSessionId, [FromBody] BookingSessionHoldDto dto)
        {
            try
            {
                var session = _service.HoldSeat(bookingSessionId, dto);
                if (session == null)
                {
                    return NotFound(new { message = "Booking session not found." });
                }

                return Ok(session);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE /api/public/booking-sessions/{bookingSessionId}/holds/{seatId}
        [HttpDelete("{bookingSessionId}/holds/{seatId}")]
        public IActionResult ReleaseSeat(string bookingSessionId, string seatId)
        {
            var session = _service.ReleaseSeat(bookingSessionId, seatId);
            if (session == null)
            {
                return NotFound(new { message = "Seat hold not found." });
            }

            return Ok(session);
        }

        // POST /api/public/booking-sessions/{bookingSessionId}/checkout
        [HttpPost("{bookingSessionId}/checkout")]
        public IActionResult MoveToCheckout(string bookingSessionId)
        {
            try
            {
                var session = _service.MoveToCheckout(bookingSessionId);
                if (session == null)
                {
                    return NotFound(new { message = "Booking session not found." });
                }

                return Ok(session);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
