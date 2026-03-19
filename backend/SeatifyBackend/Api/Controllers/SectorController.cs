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
            var sectors = await _sectorService.GetByAuditoriumAsync(auditoriumId, ct);
            return Ok(sectors);
        }

        [HttpGet("sectors/{id}")]
        public async Task<ActionResult<SectorViewDto>> GetById(string id, CancellationToken ct)
        {
            var sector = await _sectorService.GetByIdAsync(id, ct);
            if (sector == null)
            {
                return NotFound(new { message = "Sector not found" });
            }
            return Ok(sector);
        }


    }
}
