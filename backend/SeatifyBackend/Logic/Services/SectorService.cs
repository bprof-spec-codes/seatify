using Data;
using Entities.Dtos.Sector;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface ISectorService
    {
        Task<SectorViewDto> CreateAsync(string auditoriumId, SectorCreateUpdateDto dto, CancellationToken ct);
        Task<List<SectorViewDto>> GetByAuditoriumAsync(string auditoriumId, CancellationToken ct);
        Task<SectorViewDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<SectorViewDto> UpdateAsync(string id, SectorCreateUpdateDto dto, CancellationToken ct);
        Task DeleteAsync(string id, CancellationToken ct);
    }

    public class SectorService : ISectorService
    {
        private readonly AppDbContext _ctx;

        public SectorService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<SectorViewDto> CreateAsync(string auditoriumId, SectorCreateUpdateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Sector name is required.");
            }

            var auditoriumExists = await _ctx.Auditoriums.AnyAsync(a => a.Id == auditoriumId, ct);

            if (!auditoriumExists)
            {
                throw new ArgumentException("Auditorium with the specified ID does not exist.");
            }

            var normalizedName = dto.Name.Trim().ToLower();

            var duplicateExists = await _ctx.Sectors.AnyAsync(s => s.AuditoriumId == auditoriumId && s.Name.ToLower() == normalizedName, ct);

            if (duplicateExists)
            {
                throw new ArgumentException("Sector with this name already exists in this auditorium.");
            }

            var sector = new Sector
            {
                AuditoriumId = auditoriumId,
                Name = dto.Name.Trim(),
                Color = string.IsNullOrWhiteSpace(dto.Color) ? "#FFFFFF" : dto.Color.Trim(),
                BasePrice = dto.BasePrice,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _ctx.Sectors.Add(sector);
            await _ctx.SaveChangesAsync(ct);

            return new SectorViewDto
            {
                Id = sector.Id,
                AuditoriumId = sector.AuditoriumId,
                Name = sector.Name,
                Color = sector.Color,
                BasePrice = sector.BasePrice,
                CreatedAtUtc = sector.CreatedAtUtc,
                UpdatedAtUtc = sector.UpdatedAtUtc
            };
        }

        public Task DeleteAsync(string id, CancellationToken ct)
        {
            var sector = _ctx.Sectors.FirstOrDefaultAsync(s => s.Id == id, ct);
            if (sector == null)
            {
                throw new ArgumentException("Sector with the specified ID does not exist.");
            }

            _ctx.Sectors.Remove(sector.Result);
            return _ctx.SaveChangesAsync(ct);
        }

        public async Task<List<SectorViewDto>> GetByAuditoriumAsync(string auditoriumId, CancellationToken ct)
        {
            return await _ctx.Sectors
                .Where(s => s.AuditoriumId == auditoriumId)
                .OrderBy(s => s.Name)
                .Select(s => new SectorViewDto
                {
                    Id = s.Id,
                    AuditoriumId = s.AuditoriumId,
                    Name = s.Name,
                    Color = s.Color,
                    BasePrice = s.BasePrice,
                    CreatedAtUtc = s.CreatedAtUtc,
                    UpdatedAtUtc = s.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<SectorViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            return await _ctx.Sectors
                .Where(s => s.Id == id)
                .Select(s => new SectorViewDto
                {
                    Id = s.Id,
                    AuditoriumId = s.AuditoriumId,
                    Name = s.Name,
                    Color = s.Color,
                    BasePrice = s.BasePrice,
                    CreatedAtUtc = s.CreatedAtUtc,
                    UpdatedAtUtc = s.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        public Task<SectorViewDto> UpdateAsync(string id, SectorCreateUpdateDto dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
