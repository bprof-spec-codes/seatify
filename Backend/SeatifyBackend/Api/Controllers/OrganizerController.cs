using Entities.Dtos.Organizer;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/organizer")]
    public class OrganizerController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;

        public OrganizerController(IOrganizerService organizerService)
        {
            _organizerService = organizerService;
        }

        [HttpPost]
        public async Task<ActionResult<OrganizerViewDto>> CreateProfile(
            [FromBody] OrganizerCreateDto dto)
        {
            try
            {
                var result = await _organizerService.CreateAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<OrganizerViewDto>> GetMyProfile()
        {
            var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(organizerId))
            {
                return Unauthorized(new { message = "Unauthorized operation!" });
            }

            try
            {
                var result = await _organizerService.GetByIdAsync(organizerId);

                if (result == null)
                {
                    return NotFound(new { message = "Organizer profile not found." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<OrganizerViewDto>> UpdateMyProfile(
            [FromBody] OrganizerProfileUpdateDto dto)
        {
            var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(organizerId))
            {
                return Unauthorized(new { message = "Unauthorized operation!" });
            }

            try
            {
                var result = await _organizerService.UpdateProfileAsync(organizerId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizerViewDto>>> GetOrganisers()
        {
            try
            {
                var result = await _organizerService.GetAllAsync();

                if (result == null)
                {
                    return NotFound(new { message = "Organizer profile not found." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizerViewDto>> GetOrganiser([FromRoute] string id)
        {
            try
            {
                var result = await _organizerService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Organizer profile not found." });
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<OrganizerViewDto>> UpdateProfile(
            [FromRoute] string id,
            [FromBody] OrganizerUpdateDto dto)
        {
            try
            {
                var result = await _organizerService.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProfile([FromRoute] string id)
        {
            try
            {
                var success = await _organizerService.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Organizer profile not found." });
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
