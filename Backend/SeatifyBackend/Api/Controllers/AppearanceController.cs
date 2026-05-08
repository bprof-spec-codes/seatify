using Entities.Dtos.Appearance;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/appearance")]
    public class AppearanceController : ControllerBase
    {
        private readonly IAppearanceService _appearanceService;

        public AppearanceController(IAppearanceService appearanceService)
        {
            _appearanceService = appearanceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppearanceViewDto>>> GetMyAppearances(CancellationToken ct)
        {
            var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "org-id-01";

            var result = await _appearanceService.GetByOrganizerIdAsync(organizerId, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppearanceViewDto>> GetById(string id, CancellationToken ct)
        {
            var result = await _appearanceService.GetByIdAsync(id, ct);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AppearanceViewDto>> Create([FromBody] AppearanceCreateDto dto, CancellationToken ct)
        {
            var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "org-id-01";

            var result = await _appearanceService.CreateAsync(organizerId, dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AppearanceViewDto>> Update(string id, [FromBody] AppearanceCreateDto dto, CancellationToken ct)
        {
            var result = await _appearanceService.UpdateAsync(id, dto, ct);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id, CancellationToken ct)
        {
            var result = await _appearanceService.DeleteAsync(id, ct);
            
            return result switch
            {
                DeleteAppearanceResult.Success => NoContent(),
                DeleteAppearanceResult.NotFound => NotFound(),
                DeleteAppearanceResult.IsLastTheme => BadRequest(new { message = "You cannot delete the last theme." }),
                _ => StatusCode(500)
            };
        }

        [HttpPost("{id}/default")]
        public async Task<ActionResult> SetDefault(string id, CancellationToken ct)
        {
            var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "org-id-01";

            var success = await _appearanceService.SetDefaultAsync(organizerId, id, ct);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
