using Entities.Dtos.Reservation;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

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
            if (res == null) return NotFound();
            return Ok(res);
        }

        // GET /api/event-occurrences/{eventOccurrenceId}/reservations
        [HttpGet("api/event-occurrences/{eventOccurrenceId}/reservations")]
        public IActionResult GetByOccurrence(string eventOccurrenceId)
        {
            var reservations = _service.GetByOccurrenceId(eventOccurrenceId);
            return Ok(reservations);
        }

        // POST /api/event-occurrences/{eventOccurrenceId}/reservations
        [HttpPost("api/event-occurrences/{eventOccurrenceId}/reservations")]
        public IActionResult Create(string eventOccurrenceId, [FromBody] ReservationCreateDto dto)
        {
            var success = _service.CreateReservation(eventOccurrenceId, dto);
            if (!success) return BadRequest("Failed to create reservation.");
            return Ok("Reservation created successfully.");
        }

        // PUT /api/event-occurrences/{eventOccurrenceId}/reservations/{reservationId}
        [HttpPut("api/event-occurrences/reservations/{reservationId}")]
        public IActionResult Update(string reservationId, [FromBody] ReservationUpdateDto dto)
        {
            var success = _service.UpdateReservation(reservationId, dto);
            if (!success) return NotFound();
            return Ok("Reservation updated successfully.");
        }

        // DELETE /api/event-occurrences/{eventOccurrenceId}/reservations/{reservationId}
        [HttpDelete("api/event-occurrences/reservations/{reservationId}")]
        public IActionResult Delete(string reservationId)
        {
            var success = _service.DeleteReservation(reservationId);
            if (!success) return NotFound();
            return Ok("Reservation deleted successfully.");
        }
    }
}
