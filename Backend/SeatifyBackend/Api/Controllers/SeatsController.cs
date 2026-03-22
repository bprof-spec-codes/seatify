using Entities.Dtos.Seat;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet("api/layout-matrices/{matrixId}/seats")]
        public async Task<ActionResult<List<SeatViewDto>>> GetByMatrix(string matrixId, CancellationToken ct)
        {
            try
            {
                var result = await _seatService.GetByMatrixAsync(matrixId, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("api/seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> GetById(string seatId, CancellationToken ct)
        {
            var result = await _seatService.GetByIdAsync(seatId, ct);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("api/seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> Update(string seatId, [FromBody] SeatUpdateDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _seatService.UpdateAsync(seatId, dto, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
