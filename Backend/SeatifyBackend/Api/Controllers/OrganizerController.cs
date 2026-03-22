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

        [HttpPost]
        public async Task<ActionResult<OrganizerCreateDto>> CreateOrganizer(
            [FromBody] OrganizerCreateDto dto,
            CancellationToken ct)
        {
            try
            {
                var result = await _organizerService.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetOrganiser), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<OrganizerViewDto>> GetOrganisers(CancellationToken ct)
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

        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizerViewDto>>GetOrganiser(CancellationToken ct)
        {     try
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



        [HttpPut("{id}")]
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
