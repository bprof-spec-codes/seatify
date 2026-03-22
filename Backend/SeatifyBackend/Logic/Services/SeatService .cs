using Data;
using Entities.Dtos.Seat;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Services
{

    public interface ISeatService
    {
        Task<List<SeatViewDto>> GetByMatrixAsync(string matrixId, CancellationToken ct);
        Task<SeatViewDto?> GetByIdAsync(string seatId, CancellationToken ct);
        Task<SeatViewDto> UpdateAsync(string seatId, SeatUpdateDto dto, CancellationToken ct);
    }
    public class SeatService : ISeatService
    {
        private readonly AppDbContext _ctx;

        public SeatService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<SeatViewDto>> GetByMatrixAsync(string matrixId, CancellationToken ct)
        {
            var matrixExists = await _ctx.LayoutMatrices.AnyAsync(m => m.Id == matrixId, ct);
            if (!matrixExists)
            {
                throw new ArgumentException("LayoutMatrix with the specified ID does not exist.");
            }

            return await _ctx.Seats
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
            return await _ctx.Seats
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

        public async Task<SeatViewDto> UpdateAsync(string seatId, SeatUpdateDto dto, CancellationToken ct)
        {
            var seat = await _ctx.Seats.FirstOrDefaultAsync(s => s.Id == seatId, ct);
            if (seat == null)
            {
                throw new ArgumentException("Seat with the specified ID does not exist.");
            }

            if (!Enum.TryParse<SeatType>(dto.SeatType, true, out var parsedSeatType))
            {
                throw new ArgumentException("Invalid seat type.");
            }

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorExists = await _ctx.Sectors.AnyAsync(s => s.Id == dto.SectorId, ct);
                if (!sectorExists)
                {
                    throw new ArgumentException("Sector with the specified ID does not exist.");
                }
            }

            if (seat.Row < 0 || seat.Column < 0)
            {
                throw new ArgumentException("Seat coordinates must be zero or greater.");
            }

            seat.SeatLabel = string.IsNullOrWhiteSpace(dto.SeatLabel) ? null : dto.SeatLabel.Trim();
            seat.SectorId = string.IsNullOrWhiteSpace(dto.SectorId) ? null : dto.SectorId.Trim();
            seat.PriceOverride = dto.PriceOverride;
            seat.SeatType = parsedSeatType;
            seat.UpdatedAtUtc = DateTime.UtcNow;

            await _ctx.SaveChangesAsync(ct);

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
