using Data;
using Entities.Dtos.Venue;
using Entities.Models;
using Logic.Helper;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services;

public class VenueService
{
    private readonly Repository<Venue> _venueRepository;
    DtoProvider dtoProvider;

    public VenueService(Repository<Venue> venueRepository, DtoProvider dtoProvider)
    {
        _venueRepository = venueRepository;
        this.dtoProvider = dtoProvider;
    }

    public async Task<Venue> CreateVenueAsync(VenueCreateDto createDto)
    {
        var newVenue = dtoProvider.Mapper.Map<Venue>(createDto);
        _venueRepository.Add(newVenue);
        return newVenue;
    }

    public async Task<VenueViewDto> GetVenueByIdAsync(string venueId)
    {
        Venue? venue = await _venueRepository.GetAll()
            .Include(v => v.Auditoriums)
            .FirstOrDefaultAsync(e => e.Id == venueId);

        if (venue == null)
        {
            throw new Exception("Venue does not exist!");
        }

        VenueViewDto venueViewDto = dtoProvider.Mapper.Map<VenueViewDto>(venue);
        return venueViewDto;
    }
    
    public async Task<List<VenueViewDto>> GetAllVenuesAsync()
    {
        var venues = await _venueRepository.GetAll()
            .Include(v => v.Auditoriums)
            .ToListAsync();

        List<VenueViewDto> venueDtos = dtoProvider.Mapper.Map<List<VenueViewDto>>(venues);
        return venueDtos;
    }

    public async Task<Venue> UpdateVenueByIdAsync(VenueUpdateDto updateDto, string venueId)
    {
        Venue? existingVenue = await _venueRepository.GetAll().FirstOrDefaultAsync(e => e.Id == venueId);

        if (existingVenue == null)
        {
            throw new Exception("Venue does not exist!");
        }

        if (existingVenue.OrganizerId != updateDto.OrganizerId)
        {
            throw new Exception("The venue does not belong to the logged-in user!");
        }

        var updatedVenue = dtoProvider.Mapper.Map(updateDto, existingVenue);
        _venueRepository.Update(updatedVenue);
        return updatedVenue;
    }

    public async Task<bool> DeleteVenueByIdAsync(string organizerId, string venueId)
    {
        Venue? existingVenue = await _venueRepository.GetAll().FirstOrDefaultAsync(e => e.Id == venueId);

        if (existingVenue == null)
        {
            throw new Exception("Venue does not exist!");
        }

        if (existingVenue.OrganizerId != organizerId)
        {
            throw new Exception("The venue does not belong to the logged-in user!");
        }

        _venueRepository.Delete(existingVenue);
        return true;
    }

    public async Task<List<VenueViewDto>> GetVenuesByOrganizerIdAsync(string organizerId)
    {
        var venues = await _venueRepository.GetAll()
            .Where(v => v.OrganizerId == organizerId)
            .Include(v => v.Auditoriums)
            .ToListAsync();

        return dtoProvider.Mapper.Map<List<VenueViewDto>>(venues);
    }

}
