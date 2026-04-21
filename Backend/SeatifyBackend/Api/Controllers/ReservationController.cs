using Entities.Dtos.Bookings;
using Entities.Dtos.Exceptions;
using Entities.Dtos.Reservation;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Api.Controllers
{
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _service;

        public ReservationController(IReservationService service)
        {
            _service = service;
        }

        // GET /api/reservations/{id}
        [HttpGet("api/reservations/{id}")]
        public IActionResult GetById(string id)
        {
            var res = _service.GetById(id);
            if (res == null) return NotFound(new { message = "Reservation not found" });
            return Ok(res);
        }

        // GET /api/by-event-occurrences/{eventOccurrenceId}/reservations
        [HttpGet("api/by-event-occurrences/{eventOccurrenceId}/reservations")]
        public IActionResult GetByOccurrence(string eventOccurrenceId)
        {
            var reservations = _service.GetByOccurrenceId(eventOccurrenceId);
            return Ok(reservations);
        }

        // POST /api/by-event-occurrences/{eventOccurrenceId}/reservations
        [HttpPost("api/by-event-occurrences/{eventOccurrenceId}/reservations")]
        public IActionResult Create(string eventOccurrenceId, [FromBody] ReservationCreateDto dto)
        {
            var success = _service.CreateReservation(eventOccurrenceId, dto);
            if (!success) return BadRequest(new { message = "Failed to create reservation." });
            return Ok(new { message = "Reservation created successfully." });
        }

        // PUT /api/by-event-occurrences/{eventOccurrenceId}/reservations/{reservationId}
        [HttpPut("api/by-event-occurrences/reservations/{reservationId}")]
        public IActionResult Update(string reservationId, [FromBody] ReservationUpdateDto dto)
        {
            var success = _service.UpdateReservation(reservationId, dto);
            if (!success) return NotFound(new { message = "Reservation not found" });
            return Ok(new { message = "Reservation updated successfully." });
        }

        // DELETE /api/by-event-occurrences/{eventOccurrenceId}/reservations/{reservationId}
        [HttpDelete("api/by-event-occurrences/reservations/{reservationId}")]
        public IActionResult Delete(string reservationId)
        {
            var success = _service.DeleteReservation(reservationId);
            if (!success) return NotFound(new { message = "Reservation not found" });
            return Ok(new { message = "Reservation deleted successfully." });
        }
        [HttpPost("api/bookings/checkout")]
        public IActionResult CreateReservation([FromBody]BookingCheckoutRequestDto request)
        {
            try
            {
                return Ok(_service.CheckoutReservation(request));
            }
            catch(EventOccurrenceNotFoundException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (BookingSessionNotFoundException e)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = e.Message });
            }
        }
    }
}
