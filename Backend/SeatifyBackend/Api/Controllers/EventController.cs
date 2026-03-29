using Entities.Dtos.Event;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("events")]
        public async Task<ActionResult<EventViewDto>> Create(
            [FromBody] EventCreateDto dto,
            CancellationToken ct)
        {
            try
            {
                var created = await _eventService.CreateAsync(dto, ct);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("events")]
        public async Task<ActionResult<List<EventViewDto>>> GetAll(CancellationToken ct)
        {
            try
            {
                var events = await _eventService.GetAllAsync(ct);
                return Ok(events);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("events/{eventId}")]
        public async Task<ActionResult<EventViewDto>> GetById(
            string eventId,
            CancellationToken ct)
        {
            try
            {
                var ev = await _eventService.GetByIdAsync(eventId, ct);

                if (ev == null)
                {
                    return NotFound(new { message = "Event not found" });
                }

                return Ok(ev);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("events/{eventId}")]
        public async Task<ActionResult<EventViewDto>> Update(
            string eventId,
            [FromBody] EventUpdateDto dto,
            CancellationToken ct)
        {
            try
            {
                var updated = await _eventService.UpdateAsync(eventId, dto, ct);

                if (updated == null)
                {
                    return NotFound(new { message = "Event not found" });
                }

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("events/{eventId}")]
        public async Task<ActionResult> Delete(
            string eventId,
            CancellationToken ct)
        {
            try
            {
                var deleted = await _eventService.DeleteAsync(eventId, ct);

                if (!deleted)
                {
                    return NotFound(new { message = "Event not found" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}