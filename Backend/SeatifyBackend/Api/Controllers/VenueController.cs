using Entities.Dtos.Venue;
using Entities.Models;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("api")]
public class VenueController : ControllerBase
{
    private readonly VenueService _venueService;

    public VenueController(VenueService venueService)
    {
        _venueService = venueService;
    }

    [Authorize]
    [HttpPost("venues")]
    public async Task<IActionResult> CreateVenue([FromBody] VenueCreateDto venueCreateDto)
    {
        var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(organizerId))
        {
            return Unauthorized(new { message = "Unauthorized operation!" });
        }

        try
        {
            Venue newVenue = await _venueService.CreateVenueAsync(venueCreateDto, organizerId);
            return Ok(newVenue);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("venues/{id}")]
    public async Task<IActionResult> GetVenueById(string id)
    {
        try
        {
            var venue = await _venueService.GetVenueByIdAsync(id);
            return Ok(venue);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("venues")]
    public async Task<ActionResult<List<VenueViewDto>>> GetAllVenues()
    {
        var venue = await _venueService.GetAllVenuesAsync();
        return Ok(venue);
    }


    [Authorize]
    [HttpPut("venues/{id}")]
    public async Task<IActionResult> UpdateVenueById([FromBody] VenueUpdateDto venueUpdateDto, string id)
    {
        var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(organizerId))
        {
            return Unauthorized(new { message = "Unauthorized operation!" });
        }

        try
        {
            await _venueService.UpdateVenueByIdAsync(venueUpdateDto, id, organizerId);
            return Ok(venueUpdateDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("venues/{id}")]
    public async Task<IActionResult> DeleteVenueById(string id)
    {
        var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(organizerId))
        {
            return Unauthorized(new { message = "Unauthorized operation!" });
        }

        try
        {
            await _venueService.DeleteVenueByIdAsync(organizerId, id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("venues/organizers/{organizerId}")]
    public async Task<ActionResult<List<VenueViewDto>>> GetVenuesByOrganizerId(string organizerId)
    {
        try
        {
            var venues = await _venueService.GetVenuesByOrganizerIdAsync(organizerId);
            return Ok(venues);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
