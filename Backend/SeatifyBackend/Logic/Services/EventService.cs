using Data;
using Entities.Dtos.Event;
using Entities.Dtos.EventOccurrence;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface IEventService
    {
        Task<Entities.Dtos.Event.EventViewDto> CreateAsync(EventCreateDto dto, CancellationToken ct);
        Task<List<Entities.Dtos.Event.EventViewDto>> GetAllAsync(CancellationToken ct);
        Task<Entities.Dtos.Event.EventViewDto?> GetByIdAsync(string eventId, CancellationToken ct);
        Task<Entities.Dtos.Event.EventViewDto?> UpdateAsync(string eventId, EventUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string eventId, CancellationToken ct);

        Task<List<Entities.Dtos.Event.EventViewDto>> GetPublicAsync(CancellationToken ct);
        Task<List<Entities.Dtos.Event.EventViewDto>> GetByUserIdAsync(string userId, CancellationToken ct);
        Task<List<Entities.Dtos.EventOccurrence.EventOccurrenceViewDto>> GetOccurrencesByEventIdAsync(string eventId, CancellationToken ct);
        Task<Entities.Dtos.Event.EventViewDto?> GetBySlugAsync(string slug, CancellationToken ct);
        Task<bool> HasBookingsAsync(string eventId, CancellationToken ct);
    }

    public class EventService : IEventService
    {
        private readonly AppDbContext _dbContext;

        public EventService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Entities.Dtos.Event.EventViewDto> CreateAsync(EventCreateDto dto, CancellationToken ct)
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
                UpdatedAtUtc = DateTime.UtcNow,
                Appearance = new EventAppearance
                {
                    Currency = dto.Currency,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                }
            };

            _dbContext.Events.Add(entity);
            await _dbContext.SaveChangesAsync(ct);

            return MapToViewDto(entity);
        }

        public async Task<List<Entities.Dtos.Event.EventViewDto>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.Events
                .OrderBy(e => e.Name)
                .Select(e => new Entities.Dtos.Event.EventViewDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Slug = e.Slug,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                    Currency = e.Appearance != null ? e.Appearance.Currency : null,
                    CreatedAtUtc = e.CreatedAtUtc,
                    UpdatedAtUtc = e.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<Entities.Dtos.Event.EventViewDto?> GetByIdAsync(string eventId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentException("EventId is required.");
            }

            eventId = eventId.Trim();

            return await _dbContext.Events
                .Where(e => e.Id == eventId)
                .Select(e => new Entities.Dtos.Event.EventViewDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Slug = e.Slug,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                    Currency = e.Appearance != null ? e.Appearance.Currency : null,
                    CreatedAtUtc = e.CreatedAtUtc,
                    UpdatedAtUtc = e.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<Entities.Dtos.Event.EventViewDto?> UpdateAsync(string eventId, EventUpdateDto dto, CancellationToken ct)
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
                .Include(e => e.Appearance)
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

            if (entity.Appearance == null)
            {
                entity.Appearance = new EventAppearance
                {
                    EventId = entity.Id,
                    Currency = dto.Currency,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };
            }
            else
            {
                entity.Appearance.Currency = dto.Currency;
                entity.Appearance.UpdatedAtUtc = DateTime.UtcNow;
            }

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

        public async Task<List<Entities.Dtos.Event.EventViewDto>> GetPublicAsync(CancellationToken ct)
        {
            return await _dbContext.Events
                .Where(e => e.Status == "Published")
                .OrderBy(e => e.Name)
                .Select(e => new Entities.Dtos.Event.EventViewDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Slug = e.Slug,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                    Currency = e.Appearance != null ? e.Appearance.Currency : null,
                    CreatedAtUtc = e.CreatedAtUtc,
                    UpdatedAtUtc = e.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<List<Entities.Dtos.Event.EventViewDto>> GetByUserIdAsync(string userId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("UserId is required.");
            }

            userId = userId.Trim();

            return await _dbContext.Events
                .Where(e => e.OrganizerId == userId)
                .OrderBy(e => e.Name)
                .Select(e => new Entities.Dtos.Event.EventViewDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Slug = e.Slug,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                    Currency = e.Appearance != null ? e.Appearance.Currency : null,
                    CreatedAtUtc = e.CreatedAtUtc,
                    UpdatedAtUtc = e.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<List<Entities.Dtos.EventOccurrence.EventOccurrenceViewDto>> GetOccurrencesByEventIdAsync(string eventId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                throw new ArgumentException("EventId is required.");
            }

            eventId = eventId.Trim();

            return await _dbContext.EventOccurrences
                .Where(eo => eo.EventId == eventId)
                .Include(eo => eo.Event)
                .Include(eo => eo.Venue)
                .Include(eo => eo.Auditorium)
                .OrderBy(eo => eo.StartsAtUtc)
                .Select(eo => new Entities.Dtos.EventOccurrence.EventOccurrenceViewDto
                {
                    Id = eo.Id,
                    EventId = eo.EventId,
                    VenueId = eo.VenueId,
                    AuditoriumId = eo.AuditoriumId,
                    StartsAtUtc = eo.StartsAtUtc,
                    EndsAtUtc = eo.EndsAtUtc,
                    BookingOpenAtUtc = eo.BookingOpenAtUtc,
                    BookingCloseAtUtc = eo.BookingCloseAtUtc,
                    CurrencyOverride = eo.CurrencyOverride,
                    Status = eo.Status
                })
                .ToListAsync(ct);
        }

        public async Task<Entities.Dtos.Event.EventViewDto?> GetBySlugAsync(string slug, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(slug)) return null;

            return await _dbContext.Events
                .Include(e => e.Appearance)
                .Where(e => e.Slug == slug.Trim())
                .Select(e => new Entities.Dtos.Event.EventViewDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Slug = e.Slug,
                    Name = e.Name,
                    Description = e.Description,
                    Status = e.Status,
                    Currency = e.Appearance != null ? e.Appearance.Currency : null,
                    CreatedAtUtc = e.CreatedAtUtc,
                    UpdatedAtUtc = e.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        private static Entities.Dtos.Event.EventViewDto MapToViewDto(Event e)
        {
            return new Entities.Dtos.Event.EventViewDto
            {
                Id = e.Id,
                OrganizerId = e.OrganizerId,
                Slug = e.Slug,
                Name = e.Name,
                Description = e.Description,
                Status = e.Status,
                Currency = e.Appearance?.Currency,
                CreatedAtUtc = e.CreatedAtUtc,
                UpdatedAtUtc = e.UpdatedAtUtc
            };
        }

        private static List<Entities.Dtos.Event.EventViewDto> MapToViewDto(List<Event> events)
        {
            return events.Select(e => MapToViewDto(e)).ToList();
        }

        public async Task<bool> HasBookingsAsync(string eventId, CancellationToken ct)
        {
            return await _dbContext.Reservations
                .Include(r => r.EventOccurrence)
                .AnyAsync(r => r.EventOccurrence.EventId == eventId && r.Status == "Confirmed", ct);
        }
    }
}