using Data;
using Entities.Dtos.Seat;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface ISeatService
    {
        Task<List<SeatViewDto>> CreateBatchAsync(List<SeatViewDto> dtos, CancellationToken ct);
        Task<List<SeatViewDto>> GetAllAsync(CancellationToken ct);
        Task<List<SeatViewDto>> GetByMatrixAsync(string matrixId, CancellationToken ct);
        Task<SeatViewDto?> GetByIdAsync(string seatId, CancellationToken ct);
        Task<SeatViewDto?> UpdateAsync(string seatId, SeatUpdateDto dto, CancellationToken ct);
    }

    public class SeatService : ISeatService
    {
        private readonly AppDbContext _dbContext;

        public SeatService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SeatViewDto>> CreateBatchAsync(List<SeatViewDto> dtos, CancellationToken ct)
        {
            if (dtos == null || dtos.Count == 0)
            {
                throw new ArgumentException("At least one seat must be provided.");
            }

            foreach (var dto in dtos)
            {
                if (string.IsNullOrWhiteSpace(dto.MatrixId))
                {
                    throw new ArgumentException("MatrixId is required.");
                }

                if (string.IsNullOrWhiteSpace(dto.SeatType))
                {
                    throw new ArgumentException("SeatType is required.");
                }
            }

            // 1) MatrixId-k ellenőrzése
            var matrixIds = dtos
                .Select(x => x.MatrixId.Trim())
                .Distinct()
                .ToList();

            var existingMatrixIds = await _dbContext.LayoutMatrices
                .Where(x => matrixIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync(ct);

            var missingMatrixId = matrixIds.FirstOrDefault(x => !existingMatrixIds.Contains(x));
            if (missingMatrixId != null)
            {
                throw new ArgumentException($"LayoutMatrix not found: {missingMatrixId}");
            }

            // 2) SectorId-k ellenőrzése
            var sectorIds = dtos
                .Where(x => !string.IsNullOrWhiteSpace(x.SectorId))
                .Select(x => x.SectorId!.Trim())
                .Distinct()
                .ToList();

            if (sectorIds.Count > 0)
            {
                var existingSectorIds = await _dbContext.Sectors
                    .Where(x => sectorIds.Contains(x.Id))
                    .Select(x => x.Id)
                    .ToListAsync(ct);

                var missingSectorId = sectorIds.FirstOrDefault(x => !existingSectorIds.Contains(x));
                if (missingSectorId != null)
                {
                    throw new ArgumentException($"Sector not found: {missingSectorId}");
                }
            }

            // 3) Duplikált pozíciók a requesten belül
            var duplicateInRequest = dtos
                .GroupBy(x => new
                {
                    MatrixId = x.MatrixId.Trim(),
                    x.Row,
                    x.Column
                })
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateInRequest != null)
            {
                throw new ArgumentException(
                    $"Duplicate seat in request at MatrixId='{duplicateInRequest.Key.MatrixId}', Row={duplicateInRequest.Key.Row}, Column={duplicateInRequest.Key.Column}.");
            }

            // 4) Duplikált pozíciók adatbázisban
            var existingSeats = await _dbContext.Seats
                .Where(s => matrixIds.Contains(s.MatrixId))
                .Select(s => new { s.MatrixId, s.Row, s.Column })
                .ToListAsync(ct);

            foreach (var dto in dtos)
            {
                var normalizedMatrixId = dto.MatrixId.Trim();

                bool alreadyExists = existingSeats.Any(s =>
                    s.MatrixId == normalizedMatrixId &&
                    s.Row == dto.Row &&
                    s.Column == dto.Column);

                if (alreadyExists)
                {
                    throw new ArgumentException(
                        $"A seat already exists at MatrixId='{normalizedMatrixId}', Row={dto.Row}, Column={dto.Column}.");
                }
            }

            // 5) Enum parse és entitások létrehozása
            var seatsToCreate = new List<Seat>();

            foreach (var dto in dtos)
            {
                var parsedSeatType = ParseSeatType(dto.SeatType);

                var seat = new Seat
                {
                    Id = Guid.NewGuid().ToString(),
                    MatrixId = dto.MatrixId.Trim(),
                    Row = dto.Row,
                    Column = dto.Column,
                    SeatLabel = string.IsNullOrWhiteSpace(dto.SeatLabel) ? null : dto.SeatLabel.Trim(),
                    SectorId = string.IsNullOrWhiteSpace(dto.SectorId) ? null : dto.SectorId.Trim(),
                    PriceOverride = dto.PriceOverride,
                    SeatType = parsedSeatType,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                seatsToCreate.Add(seat);
            }

            _dbContext.Seats.AddRange(seatsToCreate);
            await _dbContext.SaveChangesAsync(ct);

            return seatsToCreate
                .Select(MapToViewDto)
                .ToList();
        }

        public async Task<List<SeatViewDto>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.Seats
                .OrderBy(s => s.MatrixId)
                .ThenBy(s => s.Row)
                .ThenBy(s => s.Column)
                .Select(s => new SeatViewDto
                {
                    Id = s.Id,
                    MatrixId = s.MatrixId,
                    Row = s.Row,
                    Column = s.Column,
                    SeatLabel = s.SeatLabel,
                    SectorId = s.SectorId,
                    PriceOverride = s.PriceOverride,
                    SeatType = s.SeatType.ToString(),
                    IsBookable = s.SeatType != SeatType.Aisle,
                    CreatedAtUtc = s.CreatedAtUtc,
                    UpdatedAtUtc = s.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<List<SeatViewDto>> GetByMatrixAsync(string matrixId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(matrixId))
            {
                throw new ArgumentException("MatrixId is required.");
            }

            matrixId = matrixId.Trim();

            bool matrixExists = await _dbContext.LayoutMatrices.AnyAsync(m => m.Id == matrixId, ct);

            if (!matrixExists)
            {
                throw new ArgumentException("LayoutMatrix not found.");
            }

            return await _dbContext.Seats
                .Where(s => s.MatrixId == matrixId)
                .OrderBy(s => s.Row)
                .ThenBy(s => s.Column)
                .Select(s => new SeatViewDto
                {
                    Id = s.Id,
                    MatrixId = s.MatrixId,
                    Row = s.Row,
                    Column = s.Column,
                    SeatLabel = s.SeatLabel,
                    SectorId = s.SectorId,
                    PriceOverride = s.PriceOverride,
                    SeatType = s.SeatType.ToString(),
                    IsBookable = s.SeatType != SeatType.Aisle,
                    CreatedAtUtc = s.CreatedAtUtc,
                    UpdatedAtUtc = s.UpdatedAtUtc
                })
                .ToListAsync(ct);
        }

        public async Task<SeatViewDto?> GetByIdAsync(string seatId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(seatId))
            {
                throw new ArgumentException("SeatId is required.");
            }

            seatId = seatId.Trim();

            return await _dbContext.Seats
                .Where(s => s.Id == seatId)
                .Select(s => new SeatViewDto
                {
                    Id = s.Id,
                    MatrixId = s.MatrixId,
                    Row = s.Row,
                    Column = s.Column,
                    SeatLabel = s.SeatLabel,
                    SectorId = s.SectorId,
                    PriceOverride = s.PriceOverride,
                    SeatType = s.SeatType.ToString(),
                    IsBookable = s.SeatType != SeatType.Aisle,
                    CreatedAtUtc = s.CreatedAtUtc,
                    UpdatedAtUtc = s.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<SeatViewDto?> UpdateAsync(string seatId, SeatUpdateDto dto, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(seatId))
            {
                throw new ArgumentException("SeatId is required.");
            }

            if (dto == null)
            {
                throw new ArgumentException("Request body is required.");
            }

            seatId = seatId.Trim();

            var seat = await _dbContext.Seats.FirstOrDefaultAsync(s => s.Id == seatId, ct);
            if (seat == null)
            {
                return null;
            }

            var parsedSeatType = ParseSeatType(dto.SeatType);

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorId = dto.SectorId.Trim();

                bool sectorExists = await _dbContext.Sectors.AnyAsync(s => s.Id == sectorId, ct);
                if (!sectorExists)
                {
                    throw new ArgumentException("Sector not found.");
                }

                seat.SectorId = sectorId;
            }
            else
            {
                seat.SectorId = null;
            }

            seat.SeatLabel = string.IsNullOrWhiteSpace(dto.SeatLabel) ? null : dto.SeatLabel.Trim();
            seat.PriceOverride = dto.PriceOverride;
            seat.SeatType = parsedSeatType;
            seat.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(ct);

            return MapToViewDto(seat);
        }

        private static SeatType ParseSeatType(string seatType)
        {
            if (!Enum.TryParse<SeatType>(seatType?.Trim(), true, out var parsedSeatType))
            {
                var validTypes = string.Join(", ", Enum.GetNames(typeof(SeatType)));
                throw new ArgumentException($"Invalid seat type. Valid types are: {validTypes}");
            }

            return parsedSeatType;
        }

        private static SeatViewDto MapToViewDto(Seat seat)
        {
            return new SeatViewDto
            {
                Id = seat.Id,
                MatrixId = seat.MatrixId,
                Row = seat.Row,
                Column = seat.Column,
                SeatLabel = seat.SeatLabel,
                SectorId = seat.SectorId,
                PriceOverride = seat.PriceOverride,
                SeatType = seat.SeatType.ToString(),
                IsBookable = seat.SeatType != SeatType.Aisle,
                CreatedAtUtc = seat.CreatedAtUtc,
                UpdatedAtUtc = seat.UpdatedAtUtc
            };
        }
    }
}