using Azure.Core;
using Entities.Dtos.Organizer;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/organizer")]
    [Authorize]
    public class OrganizerController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;

        public OrganizerController(IOrganizerService organizerService)
        {
            _organizerService = organizerService;
        }

        [HttpGet]
        public async Task<ActionResult<OrganizerViewDto>> GetProfile(CancellationToken ct)
        {
            try
            {
                var organizerId = GetOrganizerIdFromRequest();
                var result = await _organizerService.GetByIdAsync(organizerId, ct);

                if (result == null)
                {
                    return NotFound("Organizer profile not found.");
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<OrganizerUpdateDto>> UpdateProfile(
            [FromBody] OrganizerUpdateDto dto,
            CancellationToken ct)
        {
            try
            {
                var organizerId = GetOrganizerIdFromRequest();
                var result = await _organizerService.UpdateAsync(organizerId, dto, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetOrganizerIdFromRequest()
        {
            if (!Request.Headers.TryGetValue("X-Organizer-Id", out var organizerId))
            {
                throw new ArgumentException("Missing X-Organizer-Id header.");
            }

            var value = organizerId.ToString();

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Invalid organizer id.");
            }

            return value;
        }
    }
}
