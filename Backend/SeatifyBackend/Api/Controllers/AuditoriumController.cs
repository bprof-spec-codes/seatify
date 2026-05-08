using Entities.Dtos.Auditorium;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuditoriumController : ControllerBase
    {
        private readonly IAuditoriumService _auditoriumService;

        public AuditoriumController(IAuditoriumService auditoriumService)
        {
            _auditoriumService = auditoriumService;
        }

        [HttpGet("auditoriums")]
        public async Task<ActionResult<List<AuditoriumViewDto>>> GetAll(CancellationToken ct)
        {
            var auditoriums = await _auditoriumService.GetAllAsync(ct);
            return Ok(auditoriums);
        }

        [HttpGet("auditoriums/{auditoriumId}")]
        public async Task<ActionResult<AuditoriumViewDto>> GetById(string auditoriumId, CancellationToken ct)
        {
            var auditorium = await _auditoriumService.GetByIdAsync(auditoriumId, ct);

            if (auditorium == null)
            {
                return NotFound(new { message = "Auditorium not found" });
            }

            return Ok(auditorium);
        }

        [HttpGet("venues/{venueId}/auditoriums")]
        public async Task<ActionResult<List<AuditoriumViewDto>>> GetByVenueId(string venueId, CancellationToken ct)
        {
            var auditoriums = await _auditoriumService.GetByVenueIdAsync(venueId, ct);
            return Ok(auditoriums);
        }

        [HttpPost("venues/{venueId}/auditoriums")]
        public async Task<ActionResult<AuditoriumViewDto>> Create(
            string venueId,
            [FromBody] AuditoriumCreateDto dto,
            CancellationToken ct)
        {
            try
            {
                var created = await _auditoriumService.CreateAsync(venueId, dto, ct);

                return CreatedAtAction(
                    nameof(GetById),
                    new { auditoriumId = created.Id },
                    created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("auditoriums/{auditoriumId}")]
        public async Task<ActionResult<AuditoriumViewDto>> Update(
            string auditoriumId,
            [FromBody] AuditoriumCreateDto dto,
            CancellationToken ct)
        {
            try
            {
                var updated = await _auditoriumService.UpdateAsync(auditoriumId, dto, ct);

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

        [HttpDelete("auditoriums/{auditoriumId}")]
        public async Task<ActionResult> Delete(string auditoriumId, CancellationToken ct)
        {
            var deleted = await _auditoriumService.DeleteAsync(auditoriumId, ct);

            if (!deleted)
            {
                return NotFound(new { message = "Auditorium not found" });
            }

            return NoContent();
        }

        [HttpGet("auditoriums/{auditoriumId}/has-bookings")]
        public async Task<ActionResult<bool>> HasBookings(string auditoriumId, CancellationToken ct)
        {
            var hasBookings = await _auditoriumService.HasBookingsAsync(auditoriumId, ct);
            return Ok(hasBookings);
        }
    }
}
