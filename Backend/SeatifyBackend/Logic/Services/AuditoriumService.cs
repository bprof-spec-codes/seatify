using Data;
using Entities.Dtos.Auditorium;

namespace Logic.Services
{
    public interface IAuditoriumService
    {
        Task<AuditoriumViewDto> CreateAsync(AuditoriumCreateDto auditorium, CancellationToken cancellationToken);
        Task<IReadOnlyList<AuditoriumViewDto>> GetByVenueIdAsync(string venueId, CancellationToken cancellationToken);
        Task<AuditoriumViewDto?> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<AuditoriumViewDto?> UpdateAsync(string id, AuditoriumCreateDto request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
        Task<IReadOnlyList<AuditoriumViewDto>> GetAllAsync(CancellationToken cancellationToken);
    }

    public class AuditoriumService : IAuditoriumService
    {
        private readonly AppDbContext _ctx;

        public AuditoriumService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<AuditoriumViewDto> CreateAsync(AuditoriumCreateDto auditorium, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<AuditoriumViewDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuditoriumViewDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<AuditoriumViewDto>> GetByVenueIdAsync(string venueId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuditoriumViewDto?> UpdateAsync(string id, AuditoriumCreateDto request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
