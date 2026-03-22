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

        public async Task<SeatViewDto> CreateAsync(SeatViewDto dto)
        {
            var seat = new Seat
            {
                Id = Guid.NewGuid().ToString(),
                MatrixId = dto.MatrixId,
                Row = dto.Row,
                Column = dto.Column,
                SeatLabel = dto.SeatLabel,
                SectorId = dto.SectorId,
                PriceOverride = dto.PriceOverride,
                SeatType = Enum.TryParse<SeatType>(dto.SeatType, true, out var parsedSeatType) ? parsedSeatType : throw new ArgumentException("Invalid seat type."),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Seats.Add(seat);
            await _dbContext.SaveChangesAsync();

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

        public async Task<List<SeatViewDto>> GetByMatrixAsync(string matrixId)
        {
            var matrixExists = await _dbContext.LayoutMatrices.AnyAsync(m => m.Id == matrixId);
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
                .ToListAsync();
        }

        public async Task<SeatViewDto?> GetByIdAsync(string seatId)
        {
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
            var seat = await _dbContext.Seats.FirstOrDefaultAsync(s => s.Id == seatId);
            if (seat == null)
            {
                throw new ArgumentException("Seat not found.");
            }

            if (!Enum.TryParse<SeatType>(dto.SeatType, true, out var parsedSeatType))
            {
                throw new ArgumentException("Invalid seat type.");
            }

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorExists = await _dbContext.Sectors.AnyAsync(s => s.Id == dto.SectorId);
                if (!sectorExists)
                {
                    throw new ArgumentException("Sector not found.");
                }
            }

            seat.SeatLabel = string.IsNullOrWhiteSpace(dto.SeatLabel) ? null : dto.SeatLabel.Trim();
            seat.SectorId = string.IsNullOrWhiteSpace(dto.SectorId) ? null : dto.SectorId.Trim();
            seat.PriceOverride = dto.PriceOverride;
            seat.SeatType = parsedSeatType;
            seat.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

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
