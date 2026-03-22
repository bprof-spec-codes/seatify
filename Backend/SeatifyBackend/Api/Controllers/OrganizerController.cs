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

        //javitsd ki a controlereket a logic szolgaltatasoknak megfeleloen, es a dto-kat is, hogy a logikaban levo dto-kat hasznaljuk, ne a webapi-sakat

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
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<OrganizerViewDto>> GetOrganisers()
        {
            try
            {
                var organizerId = GetOrganizerIdFromRequest();
                var result = await _organizerService.GetAllAsync();

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
        public async Task<ActionResult<OrganizerViewDto>>GetOrganiser()
        {     try
            {
                var organizerId = GetOrganizerIdFromRequest();
                var result = await _organizerService.GetByIdAsync(organizerId);
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
            [FromBody] OrganizerUpdateDto dto)
        {
            try
            {
                var organizerId = GetOrganizerIdFromRequest();
                var result = await _organizerService.UpdateAsync(organizerId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProfile()
        {
            try
            {
                var organizerId = GetOrganizerIdFromRequest();
                var success = await _organizerService.DeleteAsync(organizerId);
                if (!success)
                {
                    return NotFound("Organizer profile not found.");
                }
                return NoContent();
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
