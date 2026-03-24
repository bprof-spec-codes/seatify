using Data;
using Entities.Dtos.LayoutMatrix;
using Entities.Models;
using Logic.Helper;
using Microsoft.EntityFrameworkCore;

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

    public class LayoutMatrixService : ILayoutMatrixService
    {

        private readonly AppDbContext _ctx;
        private readonly DtoProvider _dtoProvider;

        public LayoutMatrixService(AppDbContext ctx, DtoProvider dtoProvider)
        {
            _ctx = ctx;
            _dtoProvider = dtoProvider;
        }

        public async Task<LayoutMatrixViewDto> CreateAsync(string auditoriumId, LayoutMatrixCreateDto dto, CancellationToken ct)
        {
            await ValidateCreateAsync(auditoriumId, dto, ct);

            var entity = _dtoProvider.Mapper.Map<LayoutMatrix>(dto);

            entity.Id = Guid.NewGuid().ToString();
            entity.AuditoriumId = auditoriumId;
            entity.CreatedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            entity.Seats = new List<Seat>();

            _ctx.LayoutMatrices.Add(entity);
            await _ctx.SaveChangesAsync(ct);

            return _dtoProvider.Mapper.Map<LayoutMatrixViewDto>(entity);
        }

        public Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<List<LayoutMatrixViewDto>> GetAllByAuditoriumIdAsync(string auditoriumId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<LayoutMatrixViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<LayoutMatrixSeatMapDto?> GetSeatMapByIdAsync(string id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<LayoutMatrixViewDto?> UpdateAsync(string id, LayoutMatrixUpdateDto dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private async Task ValidateCreateAsync(string auditoriumId, LayoutMatrixCreateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(auditoriumId))
                throw new ArgumentException("AuditoriumId is required.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");

            dto.Name = dto.Name.Trim();

            if (dto.Rows <= 0)
                throw new ArgumentException("Rows must be greater than 0.");

            if (dto.Columns <= 0)
                throw new ArgumentException("Columns must be greater than 0.");

            var auditoriumExists = await _ctx.Auditoriums
                .AnyAsync(a => a.Id == auditoriumId, ct);

            if (!auditoriumExists)
                throw new KeyNotFoundException("Auditorium not found.");

            var nameExists = await _ctx.LayoutMatrices
                .AnyAsync(lm =>
                    lm.AuditoriumId == auditoriumId &&
                    lm.Name.ToLower() == dto.Name.ToLower(), ct);

            if (nameExists)
                throw new InvalidOperationException("A layout matrix with this name already exists in this auditorium.");
        }
    }
}
