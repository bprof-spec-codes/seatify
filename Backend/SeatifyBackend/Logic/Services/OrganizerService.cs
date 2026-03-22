using Data;
using Entities.Dtos.Organizer;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{
    public interface IOrganizerService
    {
        Task<OrganizerViewDto> CreateAsync(OrganizerCreateDto dto);
        Task<List<OrganizerViewDto>> GetAllAsync();
        Task<OrganizerViewDto?> GetByIdAsync(string id);
        Task<OrganizerViewDto> UpdateAsync(string id, OrganizerUpdateDto dto);
        Task<bool> DeleteAsync(string id);
    }

    public class OrganizerService : IOrganizerService
    {
        private readonly AppDbContext _dbContext;
        private readonly PasswordHasher<Organizer> _passwordHasher;

        public OrganizerService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<Organizer>();
        }
        public async Task<OrganizerViewDto> CreateAsync(OrganizerCreateDto dto)
        {
            var organizer = new Organizer
            {
                Id = Guid.NewGuid().ToString(),
                Email = dto.Email.Trim(),
                Name = dto.Name.Trim(),
                BrandColor = string.IsNullOrWhiteSpace(dto.BrandColor) ? "#FFFFFF" : dto.BrandColor.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Organizers.Add(organizer);
            await _dbContext.SaveChangesAsync();

            return new OrganizerViewDto
            {
                Id = organizer.Id,
                Email = organizer.Email,
                Name = organizer.Name,
                BrandColor = organizer.BrandColor,
                CreatedAtUtc = organizer.CreatedAtUtc,
                UpdatedAtUtc = organizer.UpdatedAtUtc
            };
        }

        public async Task<List<OrganizerViewDto>> GetAllAsync()
        {
            return await _dbContext.Organizers
                .Select(o => new OrganizerViewDto
                {
                    Id = o.Id,
                    Email = o.Email,
                    Name = o.Name,
                    BrandColor = o.BrandColor,
                    CreatedAtUtc = o.CreatedAtUtc,
                    UpdatedAtUtc = o.UpdatedAtUtc
                })
                .ToListAsync();
        }



        public async Task<OrganizerViewDto?> GetByIdAsync(string id)
        {
            return await _dbContext.Organizers
                .Where(o => o.Id == id)
                .Select(o => new OrganizerViewDto
                {
                    Id = o.Id,
                    Email = o.Email,
                    Name = o.Name,
                    BrandColor = o.BrandColor,
                    CreatedAtUtc = o.CreatedAtUtc,
                    UpdatedAtUtc = o.UpdatedAtUtc
                })
                .FirstOrDefaultAsync();
        }

        public async Task<OrganizerViewDto> UpdateAsync(string id, OrganizerUpdateDto dto)
        {
            var organizer = await _dbContext.Organizers.FirstOrDefaultAsync(o => o.Id == id);
            if (organizer == null)
            {
                throw new ArgumentException("Organizer not found.");
            }

            organizer.Name = dto.Name.Trim();
            organizer.BrandColor = string.IsNullOrWhiteSpace(dto.BrandColor)
                ? "#FFFFFF"
                : dto.BrandColor.Trim();
            organizer.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new OrganizerViewDto
            {
                Id = organizer.Id,
                Email = organizer.Email,
                Name = organizer.Name,
                BrandColor = organizer.BrandColor,
                CreatedAtUtc = organizer.CreatedAtUtc,
                UpdatedAtUtc = organizer.UpdatedAtUtc
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var organizer = await _dbContext.Organizers.FirstOrDefaultAsync(o => o.Id == id);
            if (organizer == null)
            {
                return false;
            }
            _dbContext.Organizers.Remove(organizer);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
