using Data;
using Entities.Dtos.Event;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface IEventService
    {
        Task<EventViewDto> CreateAsync(EventCreateDto dto, CancellationToken ct);
        Task<List<EventViewDto>> GetAllAsync(CancellationToken ct);
        Task<EventViewDto?> GetByIdAsync(string eventId, CancellationToken ct);
        Task<EventViewDto?> UpdateAsync(string eventId, EventUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string eventId, CancellationToken ct);
    }

    public class EventService : IEventService
    {
        private readonly AppDbContext _dbContext;

        public EventService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EventViewDto> CreateAsync(EventCreateDto dto, CancellationToken ct)
        {
            if (dto == null)
            {
                throw new ArgumentException("Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.OrganizerId))
            {
                throw new ArgumentException("OrganizerId is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Slug))
            {
                throw new ArgumentException("Slug is required.");
            }

            // Organizer létezés check
            bool organizerExists = await _dbContext.Organizers
                .AnyAsync(o => o.Id == dto.OrganizerId.Trim(), ct);

            if (!organizerExists)
            {
                throw new ArgumentException("Organizer not found.");
            }

            // Slug uniqueness check
            bool slugExists = await _dbContext.Events
                .AnyAsync(e => e.Slug == dto.Slug.Trim(), ct);

            if (slugExists)
            {
                throw new ArgumentException("Slug already exists.");
            }

            var entity = new Event
            {
                Id = Guid.NewGuid().ToString(),
                OrganizerId = dto.OrganizerId.Trim(),
                Slug = dto.Slug.Trim(),
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                Status = dto.Status,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Events.Add(entity);
            await _dbContext.SaveChangesAsync(ct);

            return MapToViewDto(entity);
        }

        public async Task<List<EventViewDto>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.Events
                .OrderBy(e => e.Name)
                .Select(e => new EventViewDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Slug = e.Slug,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                    CreatedAtUtc = e.CreatedAtUtc,
                    UpdatedAtUtc = e.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<EventViewDto?> GetByIdAsync(string eventId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentException("EventId is required.");
            }

            eventId = eventId.Trim();

            return await _dbContext.Events
                .Where(e => e.Id == eventId)
                .Select(e => new EventViewDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Slug = e.Slug,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                    CreatedAtUtc = e.CreatedAtUtc,
                    UpdatedAtUtc = e.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<EventViewDto?> UpdateAsync(string eventId, EventUpdateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentException("EventId is required.");
            }

            if (dto == null)
            {
                throw new ArgumentException("Request body is required.");
            }

            eventId = eventId.Trim();

            var entity = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == eventId, ct);

            if (entity == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                entity.Name = dto.Name.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                entity.Description = dto.Description.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Slug))
            {
                var newSlug = dto.Slug.Trim();

                bool slugExists = await _dbContext.Events
                    .AnyAsync(e => e.Slug == newSlug && e.Id != entity.Id, ct);

                if (slugExists)
                {
                    throw new ArgumentException("Slug already exists.");
                }

                entity.Slug = newSlug;
            }

            entity.Status = dto.Status;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            return MapToViewDto(entity);
        }

        public async Task<bool> DeleteAsync(string eventId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentException("EventId is required.");
            }

            eventId = eventId.Trim();

            var entity = await _dbContext.Events
                .FirstOrDefaultAsync(e => e.Id == eventId, ct);

            if (entity == null)
            {
                return false;
            }

            _dbContext.Events.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);

            return true;
        }

        private static EventViewDto MapToViewDto(Event e)
        {
            return new EventViewDto
            {
                Id = e.Id,
                OrganizerId = e.OrganizerId,
                Slug = e.Slug,
                Name = e.Name,
                Description = e.Description,
                Status = e.Status,
                CreatedAtUtc = e.CreatedAtUtc,
                UpdatedAtUtc = e.UpdatedAtUtc
            };
        }
    }
}