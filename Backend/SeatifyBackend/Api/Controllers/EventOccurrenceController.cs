using Entities.Dtos.EventOccurrence;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/event-occurrences")]
    public class EventOccurrenceController : ControllerBase
    {
        private readonly EventOccurrenceService _service;

        public EventOccurrenceController(EventOccurrenceService service)
        {
            _service = service;
        }

        // GET /api/event-occurrences/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var occurrence = _service.GetById(id);
            if (occurrence == null)
            {
                return NotFound($"EventOccurrence with ID {id} not found.");
            }
            return Ok(occurrence);
        }

        // GET /api/event-occurrences/{id}/reservations
        [HttpGet("{id}/reservations")]
        public IActionResult GetReservations(string id)
        {
            var reservations = _service.GetReservations(id);
            return Ok(reservations);
        }

        // POST /api/event-occurrences
        [HttpPost]
        public IActionResult Create([FromBody] EventOccurrenceCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isCreated = _service.Create(createDto);
            if (!isCreated)
            {
                return BadRequest("Failed to create EventOccurrence.");
            }

            return Ok("EventOccurrence created successfully.");
        }

        // PUT /api/event-occurrences/{id}
        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] EventOccurrenceCreateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isUpdated = _service.Update(id, updateDto);
            if (!isUpdated)
            {
                return NotFound($"EventOccurrence with ID {id} not found or could not be updated.");
            }

            return Ok("EventOccurrence updated successfully.");
        }

        // DELETE /api/event-occurrences/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var isDeleted = _service.Delete(id);
            if (!isDeleted)
            {
                return NotFound($"EventOccurrence with ID {id} not found.");
            }

            return Ok("EventOccurrence deleted successfully.");
        }
    }
}
