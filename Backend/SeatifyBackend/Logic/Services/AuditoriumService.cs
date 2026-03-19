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
        Task<AuditoriumViewDto?> UpdateAsync(string id, AuditoriumCreateDto dto, CancellationToken ct);
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

            if (duplicate)
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

        public async Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            Auditorium? auditorium = await _ctx.Auditoriums.FirstOrDefaultAsync(a => a.Id == id);

            if (auditorium == null)
            {
                return false;
            }

            _ctx.Auditoriums.Remove(auditorium);

            await _ctx.SaveChangesAsync(ct);

            return true;
        }

        public async Task<IReadOnlyList<AuditoriumViewDto>> GetAllAsync(CancellationToken ct)
        {
            return await _ctx.Auditoriums
                .Select(a => new AuditoriumViewDto
                {
                    Id = a.Id,
                    VenueId = a.VenueId,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedAtUtc = a.CreatedAtUtc,
                    UpdatedAtUtc = a.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<AuditoriumViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            return await _ctx.Auditoriums
                .Where(a => a.Id == id)
                .Include(a => a.LayoutMatrices)
                .Select(a => new AuditoriumViewDto
                {
                    Id = a.Id,
                    VenueId = a.VenueId,
                    Name = a.Name,
                    Description = a.Description,
                    LayoutMatrices = a.LayoutMatrices.Select(lm => new LayoutMatrix //Ezt is lekell cserélni dto-ra, de nincs még megírva
                    {
                        Id = lm.Id,
                        AuditoriumId = lm.AuditoriumId
                    }).ToList(),
                    CreatedAtUtc = a.CreatedAtUtc,
                    UpdatedAtUtc = a.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IReadOnlyList<AuditoriumViewDto>> GetByVenueIdAsync(string venueId, CancellationToken ct)
        {
            return await _ctx.Auditoriums
                .Where(a => a.VenueId == venueId)
                .Select(a => new AuditoriumViewDto
                {
                    Id = a.Id,
                    VenueId = a.VenueId,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedAtUtc = a.CreatedAtUtc,
                    UpdatedAtUtc = a.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<AuditoriumViewDto?> UpdateAsync(string id, AuditoriumCreateDto dto, CancellationToken ct)
        {
            Auditorium? auditorium = await _ctx.Auditoriums.FirstOrDefaultAsync(a => a.Id == id, ct);

            if (auditorium == null)
            {
                throw new ArgumentException("Auditorium not found");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Auditorium name is required.");
            }

            string normalizedName = dto.Name.Trim().ToLower();

            var duplicate = await _ctx.Auditoriums.AnyAsync(a => a.VenueId == dto.VenueId && a.Name.ToLower() == normalizedName && a.Id != id, ct);

            if (duplicate)
            {
                throw new ArgumentException("Auditorium with this name already exists in this venue.");
            }

            auditorium.Name = dto.Name.Trim();
            auditorium.Description = dto.Description?.Trim();
            auditorium.UpdatedAtUtc = DateTime.UtcNow;

            await _ctx.SaveChangesAsync(ct);

            return new AuditoriumViewDto
            {
                Id = auditorium.Id,
                VenueId = auditorium.VenueId,
                Name = auditorium.Name,
                Description = auditorium.Description,
                CreatedAtUtc = auditorium.CreatedAtUtc,
                UpdatedAtUtc = auditorium.UpdatedAtUtc
            };
        }
    }
}
