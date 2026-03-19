using Entities.Dtos.Sector;

namespace Logic.Services
{
    public interface MyInterface
    {
        Task<SectorViewDto> CreateAsync(string auditoriumId, SectorCreateUpdateDto dto, CancellationToken ct);
        Task<List<SectorViewDto>> GetByAuditoriumAsync(string auditoriumId, CancellationToken ct);
        Task<SectorViewDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<SectorViewDto> UpdateAsync(string id, SectorCreateUpdateDto dto, CancellationToken ct);
        Task DeleteAsync(string id, CancellationToken ct);
    }

    public class SectorService
    {

    }
}
