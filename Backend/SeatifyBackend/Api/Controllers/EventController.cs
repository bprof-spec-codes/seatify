using System.Security.Claims;
using Entities.Dtos.Event;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;
using EventDto = Entities.Dtos.Event.EventViewDto;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET /api/events
        [HttpGet]
        public async Task<ActionResult<List<Entities.Dtos.Event.EventViewDto>>> GetAll(CancellationToken ct)
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

        // GET /api/events/public
        [HttpGet("public")]
        public async Task<ActionResult<List<Entities.Dtos.Event.EventViewDto>>> GetPublic(CancellationToken ct)
        {
            try
            {
                var events = await _eventService.GetPublicAsync(ct);
                return Ok(events);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/events/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetById(string id, CancellationToken ct)
        {
            try
            {
                var ev = await _eventService.GetByIdAsync(id, ct);

                if (ev == null)
                    return NotFound(new { message = "Event not found" });

                return Ok(ev);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/events/{eventId}/eventdates
        [HttpGet("{eventId}/eventdates")]
        public async Task<ActionResult<List<Entities.Dtos.Event.EventViewDto>>> GetEventDates(
            string eventId,
            CancellationToken ct)
        {
            try
            {
                var dates = await _eventService.GetOccurrencesByEventIdAsync(eventId, ct);
                return Ok(dates);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/events/organizers/{organizerId}
        [HttpGet("organizers/{organizerId}")]
        public async Task<ActionResult<List<EventViewDto>>> GetEventsByOrganizerId(string organizerId, CancellationToken ct)
        {
            try
            {
                var events = await _eventService.GetByOrganizerIdAsync(organizerId, ct);
                return Ok(events);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/events
        [HttpPost]
        public async Task<ActionResult<Entities.Dtos.Event.EventViewDto>> Create(
            [FromBody] EventCreateDto dto,
            CancellationToken ct)
        {
            var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(organizerId))
            {
                return Unauthorized(new { message = "Unauthorized operation!" });
            }
            
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

        // PUT /api/events/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Entities.Dtos.Event.EventViewDto>> Update(
            string id,
            [FromBody] EventUpdateDto dto,
            CancellationToken ct)
        {
            try
            {
                var updated = await _eventService.UpdateAsync(id, dto, ct);

                if (updated == null)
                    return NotFound(new { message = "Event not found" });

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/events/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken ct)
        {
            try
            {
                var deleted = await _eventService.DeleteAsync(id, ct);

                if (!deleted)
                    return NotFound(new { message = "Event not found" });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("public/slug/{eventSlug}")]
        public async Task<ActionResult<EventViewDto>> GetByEventSlug(string eventSlug, CancellationToken ct)
        {
            try
            {
                var ev = await _eventService.GetBySlugAsync(eventSlug, ct);
                if (ev == null) return NotFound(new { message = "Event not found" });
                return Ok(ev);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}