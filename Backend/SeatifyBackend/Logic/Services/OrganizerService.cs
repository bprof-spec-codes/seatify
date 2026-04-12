using Data;
using Entities.Dtos.Organizer;
using Entities.Dtos.Venue;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            organizer.PasswordHash = _passwordHasher.HashPassword(organizer, dto.Password);

            _dbContext.Organizers.Add(organizer);
            await _dbContext.SaveChangesAsync();

            return new OrganizerViewDto
            {
                Id = organizer.Id,
                Email = organizer.Email,
                Name = organizer.Name,
                CreatedAtUtc = organizer.CreatedAtUtc,
                UpdatedAtUtc = organizer.UpdatedAtUtc
            };
        }

        public async Task<List<OrganizerViewDto>> GetAllAsync()
        {
            return await _dbContext.Organizers
                .Include(o => o.Venues)
                .Select(o => new OrganizerViewDto
                {
                    Id = o.Id,
                    Email = o.Email,
                    Name = o.Name,
                    Venues = o.Venues.Select(v => new VenueViewDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        City = v.City,
                        PostalCode = v.PostalCode,
                        AddressLine = v.AddressLine
                    }).ToList(),
                    CreatedAtUtc = o.CreatedAtUtc,
                    UpdatedAtUtc = o.UpdatedAtUtc
                })
                .ToListAsync();
        }

        public async Task<OrganizerViewDto?> GetByIdAsync(string id)
        {
            return await _dbContext.Organizers
                .Where(o => o.Id == id)
                .Include(o => o.Venues)
                .Select(o => new OrganizerViewDto
                {
                    Id = o.Id,
                    Email = o.Email,
                    Name = o.Name,
                    Venues = o.Venues.Select(v => new VenueViewDto //inkább mapper legyen, mert itt a venue is megjelenik, de a venue-nak nincs benne a listája az auditoriumoknak, így nem lesz teljes a venue view dto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        City = v.City,
                        PostalCode = v.PostalCode,
                        AddressLine = v.AddressLine
                    }).ToList(),
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
            organizer.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new OrganizerViewDto
            {
                Id = organizer.Id,
                Email = organizer.Email,
                Name = organizer.Name,
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
