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
        Task<OrganizerViewDto> CreateAsync(OrganizerCreateDto dto, CancellationToken ct);
        Task<List<OrganizerViewDto>> GetAllAsync(CancellationToken ct);
        Task<OrganizerViewDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<OrganizerViewDto> UpdateAsync(string id, OrganizerUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string id, CancellationToken ct);
    }

    public class OrganizerService : IOrganizerService
    {
        private readonly AppDbContext _ctx;
        private readonly PasswordHasher<Organizer> _passwordHasher;

        public OrganizerService(AppDbContext ctx)
        {
            _ctx = ctx;
            _passwordHasher = new PasswordHasher<Organizer>();
        }

        public async Task<OrganizerViewDto> CreateAsync(OrganizerCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException("Organizer email is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Organizer name is required.");
            }

            var normalizedEmail = dto.Email.Trim().ToLower();

            var duplicateExists = await _ctx.Organizers
                .AnyAsync(o => o.Email.ToLower() == normalizedEmail, ct);

            if (duplicateExists)
            {
                throw new ArgumentException("Organizer with this email already exists.");
            }

            var organizer = new Organizer
            {
                Email = dto.Email.Trim(),
                Name = dto.Name.Trim(),
                BrandColor = string.IsNullOrWhiteSpace(dto.BrandColor) ? "#FFFFFF" : dto.BrandColor.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            organizer.PasswordHash = _passwordHasher.HashPassword(organizer, dto.Password);

            _ctx.Organizers.Add(organizer);
            await _ctx.SaveChangesAsync(ct);

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

        public async Task<List<OrganizerViewDto>> GetAllAsync(CancellationToken ct)
        {
            return await _ctx.Organizers
                .OrderBy(o => o.Name)
                .Select(o => new OrganizerViewDto
                {
                    Id = o.Id,
                    Email = o.Email,
                    Name = o.Name,
                    BrandColor = o.BrandColor,
                    CreatedAtUtc = o.CreatedAtUtc,
                    UpdatedAtUtc = o.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<OrganizerViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            return await _ctx.Organizers
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
                .FirstOrDefaultAsync(ct);
        }

        public async Task<OrganizerViewDto> UpdateAsync(string id, OrganizerUpdateDto dto, CancellationToken ct)
        {
            var organizer = await _ctx.Organizers.FirstOrDefaultAsync(o => o.Id == id, ct);

            if (organizer == null)
            {
                throw new ArgumentException("Organizer with the specified ID does not exist.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Organizer name is required.");
            }

            organizer.Name = dto.Name.Trim();
            organizer.BrandColor = string.IsNullOrWhiteSpace(dto.BrandColor) ? "#FFFFFF" : dto.BrandColor.Trim();
            organizer.UpdatedAtUtc = DateTime.UtcNow;

            await _ctx.SaveChangesAsync(ct);

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

        public async Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            var organizer = await _ctx.Organizers.FirstOrDefaultAsync(o => o.Id == id, ct);

            if (organizer == null)
            {
                throw new ArgumentException("Organizer with the specified ID does not exist.");
            }

            _ctx.Organizers.Remove(organizer);
            await _ctx.SaveChangesAsync(ct);

            return true;
        }
    }
}
