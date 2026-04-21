using System.Threading.Tasks;
using Entities.Dtos.CheckIn;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Assume staff/admin should be authenticated
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost("validate")]
        public async Task<ActionResult<CheckInResult>> ValidateTicket([FromBody] CheckInValidateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Payload))
            {
                return BadRequest("Payload is required.");
            }

            var result = await _checkInService.ValidateTicketAsync(request.Payload);
            return Ok(result);
        }

        [HttpPost("confirm")]
        public async Task<ActionResult<CheckInResult>> ConfirmCheckIn([FromBody] CheckInConfirmRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.TicketId))
            {
                return BadRequest("TicketId is required.");
            }

            var result = await _checkInService.ConfirmCheckInAsync(request.TicketId);
            return Ok(result);
        }
    }
}
