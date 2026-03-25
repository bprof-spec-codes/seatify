using Entities.Dtos.LayoutMatrix;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class LayoutMatrixController : ControllerBase
    {
        private readonly ILayoutMatrixService _layoutMatrixService;

        public LayoutMatrixController(ILayoutMatrixService layoutMatrixService)
        {
            _layoutMatrixService = layoutMatrixService;
        }

        [HttpGet("auditoriums/{auditoriumId}/layout-matrices")]
        public async Task<ActionResult<List<LayoutMatrixViewDto>>> GetByAuditorium(string auditoriumId, CancellationToken ct)
        {
            try
            {
                var result = await _layoutMatrixService.GetAllByAuditoriumIdAsync(auditoriumId, ct);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("layout-matrices/{matrixId}")]
        public async Task<ActionResult<LayoutMatrixViewDto>> GetById(string matrixId, CancellationToken ct)
        {
            var result = await _layoutMatrixService.GetByIdAsync(matrixId, ct);
            if (result == null)
            {
                return NotFound(new { message = "Layout matrix not found." });
            }
            return Ok(result);
        }

        [HttpGet("layout-matrices/{matrixId}/seat-map")]
        public async Task<ActionResult<LayoutMatrixSeatMapDto>> GetSeatMapById(string matrixId, CancellationToken ct)
        {
            var result = await _layoutMatrixService.GetSeatMapByIdAsync(matrixId, ct);
            if (result == null)
            {
                return NotFound(new { message = "Layout matrix not found." });
            }
            return Ok(result);
        }

        [HttpPost("auditoriums/{auditoriumId}/layout-matrices")]
        public async Task<ActionResult<LayoutMatrixViewDto>> Create(string auditoriumId, [FromBody] LayoutMatrixCreateDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _layoutMatrixService.CreateAsync(auditoriumId, dto, ct);
                return CreatedAtAction(nameof(GetById), new { matrixId = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });

            }
        }

        [HttpPut("layout-matrices/{matrixId}")]
        public async Task<ActionResult<LayoutMatrixViewDto>> Update(string matrixId, [FromBody] LayoutMatrixUpdateDto dto, CancellationToken ct)
        {
            try
            {
                var result = await _layoutMatrixService.UpdateAsync(matrixId, dto, ct);
                if (result == null)
                {
                    return NotFound(new { message = "Layout matrix not found." });
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("layout-matrices/{matrixId}")]
        public async Task<ActionResult> Delete(string matrixId, CancellationToken ct)
        {
            var success = await _layoutMatrixService.DeleteAsync(matrixId, ct);
            if (!success)
            {
                return NotFound(new { message = "Layout matrix not found." });
            }
            return NoContent();
        }
    }
}
