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


    }
}
