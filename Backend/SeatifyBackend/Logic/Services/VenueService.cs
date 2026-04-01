using Data;
using Entities.Dtos.Venue;
using Entities.Models;
using Logic.Helper;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services;

public class VenueService
{
    private readonly AppDbContext _ctx;
    private readonly DtoProvider dtoProvider;

    public VenueService(AppDbContext ctx, DtoProvider dtoProvider)
    {
        _ctx = ctx;
        this.dtoProvider = dtoProvider;
    }

    public async Task<Venue> CreateVenueAsync(VenueCreateDto createDto)
    {
        var newVenue = dtoProvider.Mapper.Map<Venue>(createDto);

        await _ctx.Venues.AddAsync(newVenue);
        await _ctx.SaveChangesAsync();

        return newVenue;
    }

    public async Task<VenueViewDto> GetVenueByIdAsync(string venueId)
    {
        var venue = await _ctx.Venues
            .Include(v => v.Auditoriums)
            .FirstOrDefaultAsync(v => v.Id == venueId);

        if (venue == null)
        {
            throw new Exception("Venue does not exist!");
        }

        return dtoProvider.Mapper.Map<VenueViewDto>(venue);
    }

    public async Task<List<VenueViewDto>> GetAllVenuesAsync()
    {
        var venues = await _ctx.Venues
            .Include(v => v.Auditoriums)
            .ToListAsync();

        return dtoProvider.Mapper.Map<List<VenueViewDto>>(venues);
    }

    public async Task<Venue> UpdateVenueByIdAsync(VenueUpdateDto updateDto, string venueId)
    {
        var existingVenue = await _ctx.Venues
            .FirstOrDefaultAsync(v => v.Id == venueId);

        if (existingVenue == null)
        {
            throw new Exception("Venue does not exist!");
        }

        if (existingVenue.OrganizerId != updateDto.OrganizerId)
        {
            throw new Exception("The venue does not belong to the logged-in user!");
        }

        dtoProvider.Mapper.Map(updateDto, existingVenue);

        _ctx.Venues.Update(existingVenue);
        await _ctx.SaveChangesAsync();

        return existingVenue;
    }

    public async Task<bool> DeleteVenueByIdAsync(string organizerId, string venueId)
    {
        var existingVenue = await _ctx.Venues
            .FirstOrDefaultAsync(v => v.Id == venueId);

        if (existingVenue == null)
        {
            throw new Exception("Venue does not exist!");
        }

        if (existingVenue.OrganizerId != organizerId)
        {
            throw new Exception("The venue does not belong to the logged-in user!");
        }

        _ctx.Venues.Remove(existingVenue);
        await _ctx.SaveChangesAsync();

        return true;
    }

    public async Task<List<VenueViewDto>> GetVenuesByOrganizerIdAsync(string organizerId)
    {
        var venues = await _ctx.Venues
            .Where(v => v.OrganizerId == organizerId)
            .Include(v => v.Auditoriums)
            .ToListAsync();

        return dtoProvider.Mapper.Map<List<VenueViewDto>>(venues);
    }
}
