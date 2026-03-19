using Data;
using Entities.Dtos.Sector;

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
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<SectorViewDto>> GetByAuditoriumAsync(string auditoriumId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<SectorViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<SectorViewDto> UpdateAsync(string id, SectorCreateUpdateDto dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
