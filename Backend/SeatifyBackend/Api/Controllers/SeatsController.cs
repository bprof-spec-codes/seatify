using Entities.Dtos.Exceptions;
using Entities.Dtos.Seat;
using Entities.Models;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatsController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpPost("seats/batch")]
        public async Task<ActionResult<List<SeatViewDto>>> CreateBatch(
            [FromBody] List<SeatViewDto> dtos,
            CancellationToken ct)
        {
            try
            {
                var created = await _seatService.CreateBatchAsync(dtos, ct);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("seats/bulk")]
        public async Task<ActionResult<BulkSeatUpdateResponseDto>> BulkUpdate(
            [FromBody] BulkSeatUpdateDto dto,
            CancellationToken ct)
        {
            try
            {
                var response = await _seatService.BulkUpdateAsync(dto, ct);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("seats")]
        public async Task<ActionResult<List<SeatViewDto>>> GetAll(CancellationToken ct)
        {
            try
            {
                var seats = await _seatService.GetAllAsync(ct);
                return Ok(seats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("layout-matrices/{matrixId}/seats")]
        public async Task<ActionResult<List<SeatViewDto>>> GetByMatrix(
            string matrixId,
            CancellationToken ct)
        {
            try
            {
                var seats = await _seatService.GetByMatrixAsync(matrixId, ct);
                return Ok(seats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> GetById(
            string seatId,
            CancellationToken ct)
        {
            try
            {
                var seat = await _seatService.GetByIdAsync(seatId, ct);

                if (seat == null)
                {
                    return NotFound(new { message = "Seat not found" });
                }

                return Ok(seat);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> Update(
            string seatId,
            [FromBody] SeatUpdateDto dto,
            CancellationToken ct)
        {
            try
            {
                var updated = await _seatService.UpdateAsync(seatId, dto, ct);

                if (updated == null)
                {
                    return NotFound(new { message = "Seat not found" });
                }

                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("seats/{seatId}")]
        public async Task<ActionResult> Delete(
            string seatId,
            CancellationToken ct)
        {
            try
            {
                var deleted = await _seatService.DeleteAsync(seatId, ct);

                if (!deleted)
                {
                    return NotFound(new { message = "Seat not found" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("/public/events/{eventOccurrenceId}/seatmap")]
        public IActionResult GetSeatMap(string eventOccurrenceId)
        {
            try
            {
                return Ok(_seatService.GetSeatMap(eventOccurrenceId));
            }
            catch (EventNotFoundException e)
            {
                return NotFound(new {message = e.Message});
            }
        }

[HttpPost("events/seat-availability")]
public IActionResult GetSeatAvailability([FromBody] SeatAvailabilityRequestDto requestDto)
{
    try
    {
        SeatAvailabilityResponseDto responseDto = _seatService.getSeatAvailability(requestDto);
        if (responseDto.valid)
        {
            return Ok(responseDto);
        }
        else
        {
            return Conflict(responseDto);
        }
    }
    catch (EventNotFoundException e)
    {
        return BadRequest(new { message = e.Message });
    }
}
    }
}