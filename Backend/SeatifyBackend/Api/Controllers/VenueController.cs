using System.Security.Claims;
using Entities.Dtos.Venue;
using Entities.Models;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenueController : ControllerBase
{
    private readonly VenueService _venueService;

    public VenueController(VenueService venueService)
    {
        _venueService = venueService;
    }

    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateVenue([FromBody] VenueCreateDto venueCreateDto)
    {
        var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        /* (organizerId == null)
        {
            return Unauthorized(new { message = "Unauthorized operation!" });
        }*/

        try
        {
            //venueCreateDto.OrganizerId = organizerId;
            venueCreateDto.OrganizerId = organizerId ?? "";
        
            Venue newVenue = await _venueService.CreateVenueAsync(venueCreateDto);
            return Ok(newVenue);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
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
    
    [HttpGet]
    public async Task<ActionResult<List<VenueViewDto>>> GetAllVenues()
    {
        var auditoriums = await _venueService.GetAllVenuesAsync();
        return Ok(auditoriums);
    }
    
    //[Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVenueById([FromBody] VenueUpdateDto venueUpdateDto, string id)
    {
        var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        /*if (organizerId == null)
        {
            return Unauthorized(new { message = "Unauthorized operation!" });
        }*/
        
        try
        {
            //venueUpdateDto.OrganizerId = organizerId;
            venueUpdateDto.OrganizerId = organizerId ?? "";

            await _venueService.UpdateVenueByIdAsync(venueUpdateDto, id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    //[Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVenueById(string id)
    {
        var organizerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        /*if (organizerId == null)
        {
            return Unauthorized(new { message = "Unauthorized operation!" });
        }*/
        
        try
        {
            //await _venueService.DeleteVenueByIdAsync(organizerId, id);
            await _venueService.DeleteVenueByIdAsync(organizerId ?? "", id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
