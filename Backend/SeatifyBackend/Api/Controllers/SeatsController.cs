using Entities.Dtos.Seat;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly SeatService _logic;

        public SeatsController(SeatService logic)
        {
            _logic = logic;
        }

        [HttpPost("api/seats/batch")]
        public async Task<ActionResult<IEnumerable<SeatViewDto>>> CreateBatch([FromBody] List<SeatViewDto> dtos)
        {
            var results = new List<SeatViewDto>();
            foreach (var dto in dtos)
            {
                results.Add(await _logic.CreateAsync(dto));
            }
            return Ok(results);
        }

        [HttpGet("api/layout-matrices/{matrixId}/seats")]
        public async Task<ActionResult<IEnumerable<SeatViewDto>>> GetByMatrix([FromRoute] string matrixId)
        {
            return Ok(await _logic.GetByMatrixAsync(matrixId));
        }

        [HttpGet("api/seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> GetById([FromRoute] string seatId)
        {
            return Ok(await _logic.GetByIdAsync(seatId));
        }

        [HttpPut("api/seats/{seatId}")]
        public async Task<ActionResult<SeatViewDto>> Update([FromRoute] string seatId, [FromBody] SeatUpdateDto dto)
        {
            return Ok(await _logic.UpdateAsync(seatId, dto));
        }
    }
}
