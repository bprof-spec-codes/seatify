using Entities.Dtos.Seat;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _logic;

        public SeatsController(ISeatService logic)
        {
            _logic = logic;
        }

        [HttpPost("api/seats/batch")]
        public async Task<ActionResult<IEnumerable<SeatViewDto>>> CreateBatch([FromBody] List<SeatCreateDto> dtos)
        {
            try
            {
                var result = await _logic.CreateBatchAsync(dtos);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("api/layout-matrices/{matrixId}/seats")]
        public async Task<ActionResult<IEnumerable<SeatViewDto>>> GetByMatrix([FromRoute] string matrixId)
        {
            try
            {
                var result = await _logic.GetByMatrixAsync(matrixId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("api/seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> GetById([FromRoute] string seatId)
        {
            try
            {
                var result = await _logic.GetByIdAsync(seatId);

                if (result == null)
                {
                    return NotFound(new { message = "Seat not found." });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("api/seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> Update([FromRoute] string seatId, [FromBody] SeatUpdateDto dto)
        {
            try
            {
                var result = await _logic.UpdateAsync(seatId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}