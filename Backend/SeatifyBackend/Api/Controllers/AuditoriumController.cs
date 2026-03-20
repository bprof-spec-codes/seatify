using Entities.Dtos.Auditorium;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriumController : ControllerBase
    {
        private readonly IAuditoriumService _auditoriumService;

        public AuditoriumController(IAuditoriumService auditoriumService)
        {
            _auditoriumService = auditoriumService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuditoriumViewDto>>> GetAll(CancellationToken ct)
        {
            var auditoriums = await _auditoriumService.GetAllAsync(ct);
            return Ok(auditoriums);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuditoriumViewDto>> GetById(string id, CancellationToken ct)
        {
            var auditorium = await _auditoriumService.GetByIdAsync(id, ct);
            if (auditorium == null)
            {
                return NotFound(new { message = "Auditorium not found" });
            }
            return Ok(auditorium);
        }

        [HttpGet("venue/{venueId}")]
        public async Task<ActionResult<List<AuditoriumViewDto>>> GetByVenueId(string venueId, CancellationToken ct)
        {
            var auditoriums = await _auditoriumService.GetByVenueIdAsync(venueId, ct);
            return Ok(auditoriums);
        }

        [HttpPost]
        public async Task<ActionResult<AuditoriumViewDto>> Create([FromBody] AuditoriumCreateDto dto, CancellationToken ct)
        {
            try
            {
                var created = await _auditoriumService.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuditoriumViewDto>> Update(string id, [FromBody] AuditoriumCreateDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await _auditoriumService.UpdateAsync(id, dto, ct);
                if (updated == null)
                {
                    return NotFound(new { message = "Auditorium not found" });
                }
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken ct)
        {
            var deleted = await _auditoriumService.DeleteAsync(id, ct);
            if (!deleted)
            {
                return NotFound(new { message = "Auditorium not found" });
            }
            return NoContent();
        }
    }
}
