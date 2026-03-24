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
        public async Task<ActionResult<List<Entities.Dtos.LayoutMatrix.LayoutMatrixViewDto>>> GetByAuditorium(string auditoriumId, CancellationToken ct)
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
    }
}
