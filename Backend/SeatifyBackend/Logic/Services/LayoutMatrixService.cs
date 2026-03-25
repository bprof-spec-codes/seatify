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
            entity.Seats = GenerateSeats(entity.Id, entity.Rows, entity.Columns, entity.CreatedAtUtc);

            _ctx.LayoutMatrices.Add(entity);
            await _ctx.SaveChangesAsync(ct);

            return _dtoProvider.Mapper.Map<LayoutMatrixViewDto>(entity);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            var entity = await _ctx.LayoutMatrices
                .Include(lm => lm.Seats)
                .FirstOrDefaultAsync(lm => lm.Id == id, ct);

            if (entity == null)
            {
                return false;
            }

            _ctx.Seats.RemoveRange(entity.Seats);
            _ctx.LayoutMatrices.Remove(entity);

            await _ctx.SaveChangesAsync(ct);
            return true;
        }

        public async Task<List<LayoutMatrixViewDto>> GetAllByAuditoriumIdAsync(string auditoriumId, CancellationToken ct)
        {
            var entities = await _ctx.LayoutMatrices
                .AsNoTracking()
                .Where(lm => lm.AuditoriumId == auditoriumId)
                .OrderBy(lm => lm.Name)
                .ToListAsync(ct);

            return entities.Select(e => _dtoProvider.Mapper.Map<LayoutMatrixViewDto>(e)).ToList();
        }

        public async Task<LayoutMatrixViewDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            var entity = await _ctx.LayoutMatrices
                .AsNoTracking()
                .FirstOrDefaultAsync(lm => lm.Id == id, ct);

            if (entity == null)
            {
                return null;
            }

            return _dtoProvider.Mapper.Map<LayoutMatrixViewDto>(entity);
        }

        public async Task<LayoutMatrixSeatMapDto?> GetSeatMapByIdAsync(string id, CancellationToken ct)
        {
            var entity = await _ctx.LayoutMatrices
                .AsNoTracking()
                .Include(lm => lm.Seats)
                .FirstOrDefaultAsync(lm => lm.Id == id, ct);

            if (entity == null)
            {
                return null;
            }

            entity.Seats = entity.Seats
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Column)
                .ToList();

            return _dtoProvider.Mapper.Map<LayoutMatrixSeatMapDto>(entity);
        }

        public async Task<LayoutMatrixViewDto?> UpdateAsync(string id, LayoutMatrixUpdateDto dto, CancellationToken ct)
        {
            var entity = await _ctx.LayoutMatrices
                .Include(lm => lm.Seats)
                .FirstOrDefaultAsync(lm => lm.Id == id, ct);

            if (entity == null)
            {
                return null;
            }

            await ValidateUpdateAsync(entity, dto, ct);

            var oldRows = entity.Rows;
            var oldColumns = entity.Columns;

            _dtoProvider.Mapper.Map(dto, entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            var dimensionsChanged = oldRows != dto.Rows || oldColumns != dto.Columns;

            if (dimensionsChanged)
            {
                _ctx.Seats.RemoveRange(entity.Seats);

                entity.Seats = GenerateSeats(entity.Id, entity.Rows, entity.Columns, entity.UpdatedAtUtc);
            }

            await _ctx.SaveChangesAsync(ct);

            return _dtoProvider.Mapper.Map<LayoutMatrixViewDto>(entity);
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

        private async Task ValidateUpdateAsync(LayoutMatrix entity, LayoutMatrixUpdateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");

            dto.Name = dto.Name.Trim();

            if (dto.Rows <= 0)
                throw new ArgumentException("Rows must be greater than 0.");

            if (dto.Columns <= 0)
                throw new ArgumentException("Columns must be greater than 0.");

            var nameExists = await _ctx.LayoutMatrices
                .AnyAsync(lm =>
                    lm.Id != entity.Id &&
                    lm.AuditoriumId == entity.AuditoriumId &&
                    lm.Name.ToLower() == dto.Name.ToLower(), ct);

            if (nameExists)
                throw new InvalidOperationException("A layout matrix with this name already exists in this auditorium.");
        }

        private List<Seat> GenerateSeats(string layoutMatrixId, int rows, int columns, DateTime now)
        {
            var seats = new List<Seat>();

            for (int row = 1; row <= rows; row++)
            {
                for (int column = 1; column <= columns; column++)
                {
                    seats.Add(new Seat
                    {
                        Id = Guid.NewGuid().ToString(),
                        MatrixId = layoutMatrixId,
                        Row = row,
                        Column = column,
                        SeatLabel = $"{GetRowLabel(row)}{column}",
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    });
                }
            }

            return seats;
        }

        private static string GetRowLabel(int rowNumber)
        {
            var label = string.Empty;

            while (rowNumber > 0)
            {
                rowNumber--;
                label = (char)('A' + (rowNumber % 26)) + label;
                rowNumber /= 26;
            }

            return label;
        }
    }
}
