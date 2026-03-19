using Entities.Dtos.Sector;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class SectorController : ControllerBase
    {
        private readonly ISectorService _sectorService;

        public SectorController(ISectorService sectorService)
        {
            _sectorService = sectorService;
        }

        [HttpGet("auditoriums/{auditoriumId}/sectors")]
        public async Task<ActionResult<List<SectorViewDto>>> GetByAuditorium(string auditoriumId, CancellationToken ct)
        {
            var result = await _sectorService.GetByAuditoriumAsync(auditoriumId, ct);
            return Ok(result);
        }

        [HttpGet("sectors/{id}")]
        public async Task<ActionResult<SectorViewDto>> GetById(string id, CancellationToken ct)
        {
            var result = await _sectorService.GetByIdAsync(id, ct);
            if (result == null)
            {
                return NotFound(new { message = "Sector not found" });
            }
            return Ok(result);
        }

        [HttpPost("auditoriums/{auditoriumId}/sectors")]
        public async Task<ActionResult<SectorViewDto>> Create(string auditoriumId, [FromBody] SectorCreateUpdateDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _sectorService.CreateAsync(auditoriumId, dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
