using Entities.Dtos.LayoutMatrix;

namespace Logic.Services
{
    public interface ILayoutMatrixService
    {
        Task<List<LayoutMatrixViewDto>> GetAllByAuditoriumIdAsync(string auditoriumId, CancellationToken ct);
        Task<LayoutMatrixViewDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<LayoutMatrixSeatMapDto?> GetSeatMapByIdAsync(string id, CancellationToken ct);
        Task<LayoutMatrixViewDto> CreateAsync(string auditoriumId, LayoutMatrixCreateDto dto, CancellationToken ct);
        Task<LayoutMatrixViewDto?> UpdateAsync(string id, LayoutMatrixUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string id, CancellationToken ct);
    }

    public class LayoutMatrixService
    {
    }
}
