using Data;
using Entities.Dtos.Seat;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface ISeatService
    {
        Task<List<SeatViewDto>> CreateBatchAsync(List<SeatCreateDto> dtos);
        Task<List<SeatViewDto>> GetByMatrixAsync(string matrixId);
        Task<SeatViewDto?> GetByIdAsync(string seatId);
        Task<SeatViewDto> UpdateAsync(string seatId, SeatUpdateDto dto);
    }

    public class SeatService : ISeatService
    {
        private readonly AppDbContext _dbContext;

        public SeatService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SeatViewDto>> CreateBatchAsync(List<SeatCreateDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                throw new ArgumentException("At least one seat must be provided.");
            }

            var duplicateInRequest = dtos
                .GroupBy(x => new { x.MatrixId, x.Row, x.Column })
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateInRequest != null)
            {
                throw new ArgumentException(
                    $"Duplicate seat position in request for MatrixId='{duplicateInRequest.Key.MatrixId}', Row={duplicateInRequest.Key.Row}, Column={duplicateInRequest.Key.Column}.");
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var createdSeats = new List<Seat>();

            foreach (var dto in dtos)
            {
                await ValidateCreateDtoAsync(dto);

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

                _dbContext.Seats.Add(seat);
                createdSeats.Add(seat);
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return createdSeats.Select(MapToViewDto).ToList();
        }

        public async Task<List<SeatViewDto>> GetByMatrixAsync(string matrixId)
        {
            if (string.IsNullOrWhiteSpace(matrixId))
            {
                throw new ArgumentException("MatrixId is required.");
            }

            matrixId = matrixId.Trim();

            var matrixExists = await _dbContext.LayoutMatrices.AnyAsync(m => m.Id == matrixId);
            if (!matrixExists)
            {
                throw new KeyNotFoundException("LayoutMatrix not found.");
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
                .ToListAsync();
        }

        public async Task<SeatViewDto?> GetByIdAsync(string seatId)
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
                .FirstOrDefaultAsync();
        }

        public async Task<SeatViewDto> UpdateAsync(string seatId, SeatUpdateDto dto)
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

            var seat = await _dbContext.Seats.FirstOrDefaultAsync(s => s.Id == seatId);
            if (seat == null)
            {
                throw new KeyNotFoundException("Seat not found.");
            }

            var parsedSeatType = ParseSeatType(dto.SeatType);

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorId = dto.SectorId.Trim();

                var sectorExists = await _dbContext.Sectors.AnyAsync(s => s.Id == sectorId);
                if (!sectorExists)
                {
                    throw new KeyNotFoundException("Sector not found.");
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

            await _dbContext.SaveChangesAsync();

            return MapToViewDto(seat);
        }

        private async Task ValidateCreateDtoAsync(SeatCreateDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Seat payload is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.MatrixId))
            {
                throw new ArgumentException("MatrixId is required.");
            }

            var matrixId = dto.MatrixId.Trim();

            var matrixExists = await _dbContext.LayoutMatrices.AnyAsync(m => m.Id == matrixId);
            if (!matrixExists)
            {
                throw new KeyNotFoundException("LayoutMatrix not found.");
            }

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorId = dto.SectorId.Trim();

                var sectorExists = await _dbContext.Sectors.AnyAsync(s => s.Id == sectorId);
                if (!sectorExists)
                {
                    throw new KeyNotFoundException("Sector not found.");
                }
            }

            var seatExists = await _dbContext.Seats.AnyAsync(s =>
                s.MatrixId == matrixId &&
                s.Row == dto.Row &&
                s.Column == dto.Column);

            if (seatExists)
            {
                throw new ArgumentException(
                    $"A seat already exists at MatrixId='{matrixId}', Row={dto.Row}, Column={dto.Column}.");
            }
        }

        private static SeatType ParseSeatType(string seatType)
        {
            if (string.IsNullOrWhiteSpace(seatType))
            {
                throw new ArgumentException("SeatType is required.");
            }

            if (!Enum.TryParse<SeatType>(seatType.Trim(), true, out var parsedSeatType))
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