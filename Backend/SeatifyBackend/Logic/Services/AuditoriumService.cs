using Data;
using Entities.Dtos.Auditorium;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface IAuditoriumService
    {
        Task<AuditoriumViewDto> CreateAsync(AuditoriumCreateDto dto, CancellationToken ct);
        Task<IReadOnlyList<AuditoriumViewDto>> GetByVenueIdAsync(string venueId, CancellationToken ct);
        Task<AuditoriumViewDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<AuditoriumViewDto?> UpdateAsync(string id, AuditoriumCreateDto request, CancellationToken ct);
        Task<bool> DeleteAsync(string id, CancellationToken ct);
        Task<IReadOnlyList<AuditoriumViewDto>> GetAllAsync(CancellationToken ct);
    }

    public class AuditoriumService : IAuditoriumService
    {
        private readonly AppDbContext _ctx;

        public AuditoriumService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<AuditoriumViewDto> CreateAsync(AuditoriumCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Auditorium name is required.");
            }

            bool venueExists = await _ctx.Venues.AnyAsync(v => v.Id == dto.VenueId, ct);

            if (!venueExists)
            {
                throw new ArgumentException("Venue with the specified ID does not exist.");
            }

            var duplicate = await _ctx.Auditoriums.AnyAsync(a => a.VenueId == dto.VenueId && a.Name.ToLower() == dto.Name.ToLower(), ct);

            if (!duplicate)
            {
                throw new ArgumentException("Auditorium with this name already exists in this venue.");
            }

            var auditorium = new Auditorium
            {
                VenueId = dto.VenueId,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _ctx.Auditoriums.Add(auditorium);

            await _ctx.SaveChangesAsync(ct);

            return new AuditoriumViewDto
            {
                Id = auditorium.Id,
                VenueId = auditorium.VenueId,
                Name = auditorium.Name,
                Description = auditorium.Description,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
        }

        public Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<AuditoriumViewDto>> GetAllAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<AuditoriumViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<AuditoriumViewDto>> GetByVenueIdAsync(string venueId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<AuditoriumViewDto?> UpdateAsync(string id, AuditoriumCreateDto request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
