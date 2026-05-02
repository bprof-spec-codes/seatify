using Data;
using Entities.Dtos.Appearance;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface IAppearanceService
    {
        Task<List<AppearanceViewDto>> GetByOrganizerIdAsync(string organizerId, CancellationToken ct);
        Task<AppearanceViewDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<AppearanceViewDto> CreateAsync(string organizerId, AppearanceCreateDto dto, CancellationToken ct);
        Task<AppearanceViewDto?> UpdateAsync(string id, AppearanceCreateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string id, CancellationToken ct);
        Task<bool> SetDefaultAsync(string organizerId, string id, CancellationToken ct);
    }

    public class AppearanceService : IAppearanceService
    {
        private readonly AppDbContext _dbContext;

        public AppearanceService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AppearanceViewDto>> GetByOrganizerIdAsync(string organizerId, CancellationToken ct)
        {
            return await _dbContext.Appearances
                .Where(a => a.OrganizerId == organizerId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.Name)
                .Select(a => MapToViewDto(a))
                .ToListAsync(ct);
        }

        public async Task<AppearanceViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            var appearance = await _dbContext.Appearances
                .FirstOrDefaultAsync(a => a.Id == id, ct);
            
            return appearance == null ? null : MapToViewDto(appearance);
        }

        public async Task<AppearanceViewDto> CreateAsync(string organizerId, AppearanceCreateDto dto, CancellationToken ct)
        {
            if (dto.IsDefault)
            {
                await ResetDefaultsAsync(organizerId, ct);
            }

            var appearance = new Appearance
            {
                Id = Guid.NewGuid().ToString(),
                OrganizerId = organizerId,
                Name = dto.Name,
                PrimaryColor = dto.PrimaryColor,
                AccentColor = dto.AccentColor,
                BackgroundColor = dto.BackgroundColor,
                SurfaceColor = dto.SurfaceColor,
                TextColor = dto.TextColor,
                SecondaryColor = dto.SecondaryColor,
                LogoImageUrl = dto.LogoImageUrl,
                BannerImageUrl = dto.BannerImageUrl,
                ThemePreset = dto.ThemePreset,
                IsDefault = dto.IsDefault,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Appearances.Add(appearance);
            await _dbContext.SaveChangesAsync(ct);

            return MapToViewDto(appearance);
        }

        public async Task<AppearanceViewDto?> UpdateAsync(string id, AppearanceCreateDto dto, CancellationToken ct)
        {
            var appearance = await _dbContext.Appearances
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (appearance == null) return null;

            if (dto.IsDefault && !appearance.IsDefault)
            {
                await ResetDefaultsAsync(appearance.OrganizerId, ct);
            }

            appearance.Name = dto.Name;
            appearance.PrimaryColor = dto.PrimaryColor;
            appearance.AccentColor = dto.AccentColor;
            appearance.BackgroundColor = dto.BackgroundColor;
            appearance.SurfaceColor = dto.SurfaceColor;
            appearance.TextColor = dto.TextColor;
            appearance.SecondaryColor = dto.SecondaryColor;
            appearance.LogoImageUrl = dto.LogoImageUrl;
            appearance.BannerImageUrl = dto.BannerImageUrl;
            appearance.ThemePreset = dto.ThemePreset;
            appearance.IsDefault = dto.IsDefault;
            appearance.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            return MapToViewDto(appearance);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            var appearance = await _dbContext.Appearances
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (appearance == null) return false;

            _dbContext.Appearances.Remove(appearance);
            await _dbContext.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> SetDefaultAsync(string organizerId, string id, CancellationToken ct)
        {
            var appearance = await _dbContext.Appearances
                .FirstOrDefaultAsync(a => a.Id == id && a.OrganizerId == organizerId, ct);

            if (appearance == null) return false;

            await ResetDefaultsAsync(organizerId, ct);
            
            appearance.IsDefault = true;
            appearance.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);
            return true;
        }

        private async Task ResetDefaultsAsync(string organizerId, CancellationToken ct)
        {
            var defaults = await _dbContext.Appearances
                .Where(a => a.OrganizerId == organizerId && a.IsDefault)
                .ToListAsync(ct);

            foreach (var d in defaults)
            {
                d.IsDefault = false;
                d.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        private static AppearanceViewDto MapToViewDto(Appearance a)
        {
            return new AppearanceViewDto
            {
                Id = a.Id,
                OrganizerId = a.OrganizerId,
                Name = a.Name,
                PrimaryColor = a.PrimaryColor,
                AccentColor = a.AccentColor,
                BackgroundColor = a.BackgroundColor,
                SurfaceColor = a.SurfaceColor,
                TextColor = a.TextColor,
                SecondaryColor = a.SecondaryColor,
                LogoImageUrl = a.LogoImageUrl,
                BannerImageUrl = a.BannerImageUrl,
                ThemePreset = a.ThemePreset,
                IsDefault = a.IsDefault,
                CreatedAtUtc = a.CreatedAtUtc,
                UpdatedAtUtc = a.UpdatedAtUtc
            };
        }
    }
}
