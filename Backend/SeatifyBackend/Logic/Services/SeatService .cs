using Data;
using Entities.Dtos.Exceptions;
using Entities.Dtos.Seat;
using Entities.Dtos.SeatMap;
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
        Task<bool> DeleteAsync(string seatId, CancellationToken ct);
        Task<BulkSeatUpdateResponseDto> BulkUpdateAsync(BulkSeatUpdateDto dto, CancellationToken ct);
        SeatMapDto GetSeatMap(string eventOccurrenceId);
        SeatAvailabilityResponseDto getSeatAvailability(SeatAvailabilityRequestDto SeatavailabilityRequestDto);
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

        public async Task<BulkSeatUpdateResponseDto> BulkUpdateAsync(BulkSeatUpdateDto dto, CancellationToken ct)
        {
            if (dto == null || dto.SeatIds == null || !dto.SeatIds.Any())
            {
                throw new ArgumentException("At least one SeatId must be provided.");
            }

            var uniqueSeatIds = dto.SeatIds.Select(sid => sid.Trim()).Distinct().ToList();

            var seatsToUpdate = await _dbContext.Seats
                .Where(s => uniqueSeatIds.Contains(s.Id))
                .ToListAsync(ct);

            if (seatsToUpdate.Count != uniqueSeatIds.Count)
            {
                var foundIds = seatsToUpdate.Select(s => s.Id).ToList();
                var missingIds = uniqueSeatIds.Except(foundIds).ToList();
                var missingIdsStr = string.Join(", ", missingIds);
                throw new ArgumentException($"Some seats were not found: {missingIdsStr}");
            }

            var distinctMatrixIds = seatsToUpdate.Select(s => s.MatrixId).Distinct().ToList();
            if (distinctMatrixIds.Count > 1)
            {
                throw new ArgumentException("All selected seats must belong to the same layout matrix.");
            }

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorId = dto.SectorId.Trim();
                bool sectorExists = await _dbContext.Sectors.AnyAsync(s => s.Id == sectorId, ct);
                if (!sectorExists)
                {
                    throw new ArgumentException($"Sector not found: {sectorId}");
                }
            }

            SeatType? parsedSeatType = null;
            if (!string.IsNullOrWhiteSpace(dto.SeatType))
            {
                parsedSeatType = ParseSeatType(dto.SeatType);
            }

            var updatedIds = new List<string>();

            foreach (var seat in seatsToUpdate)
            {
                bool isModified = false;

                if (!string.IsNullOrWhiteSpace(dto.SectorId))
                {
                    seat.SectorId = dto.SectorId.Trim();
                    isModified = true;
                }
                else if (dto.ClearSector)
                {
                    seat.SectorId = null;
                    isModified = true;
                }

                if (parsedSeatType.HasValue)
                {
                    seat.SeatType = parsedSeatType.Value;
                    isModified = true;
                }

                if (dto.PriceOverride.HasValue)
                {
                    seat.PriceOverride = dto.PriceOverride.Value;
                    isModified = true;
                }
                else if (dto.ClearPriceOverride)
                {
                    seat.PriceOverride = null;
                    isModified = true;
                }

                if (isModified)
                {
                    seat.UpdatedAtUtc = DateTime.UtcNow;
                    updatedIds.Add(seat.Id);
                }
            }

            await _dbContext.SaveChangesAsync(ct);

            return new BulkSeatUpdateResponseDto
            {
                UpdatedCount = updatedIds.Count,
                UpdatedSeatIds = updatedIds
            };
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

        public async Task<bool> DeleteAsync(string seatId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(seatId))
            {
                throw new ArgumentException("SeatId is required.");
            }

            seatId = seatId.Trim();

            var seat = await _dbContext.Seats.FirstOrDefaultAsync(s => s.Id == seatId, ct);
            if (seat == null)
            {
                return false;
            }

            _dbContext.Seats.Remove(seat);
            await _dbContext.SaveChangesAsync(ct);

            return true;
        }

        public SeatMapDto GetSeatMap(string eventOccurrenceId)
        {
            if (string.IsNullOrWhiteSpace(eventOccurrenceId))
            {
                throw new ArgumentException("eventOccurrenceId is required.");
            }

            eventOccurrenceId = eventOccurrenceId.Trim();

            var eventOccurrence = _dbContext.EventOccurrences
                .AsNoTracking()
                .FirstOrDefault(e => e.Id == eventOccurrenceId);

            if (eventOccurrence == null)
            {
                throw new EventNotFoundException("Invalid eventOccurrenceId: " + eventOccurrenceId);
            }

            var matrixIds = _dbContext.LayoutMatrices
                .AsNoTracking()
                .Where(m => m.AuditoriumId == eventOccurrence.AuditoriumId)
                .Select(m => m.Id)
                .ToList();

            var seats = _dbContext.Seats
                .AsNoTracking()
                .Include(s => s.Sector)
                .Where(s => matrixIds.Contains(s.MatrixId))
                .OrderBy(s => s.MatrixId)
                .ThenBy(s => s.Row)
                .ThenBy(s => s.Column)
                .ToList();

            var seatIds = seats.Select(s => s.Id).ToList();

            var eventSeatOverrides = _dbContext.EventSeatOverrides
                .AsNoTracking()
                .Include(o => o.Sector)
                .Where(o => o.EventId == eventOccurrence.EventId && seatIds.Contains(o.SeatId))
                .ToList();

            var occurrenceSeatOverrides = _dbContext.OccurrenceSeatOverrides
                .AsNoTracking()
                .Include(o => o.Sector)
                .Where(o => o.OccurrenceId == eventOccurrenceId && seatIds.Contains(o.SeatId))
                .ToList();

            var reservations = _dbContext.Reservations
                .AsNoTracking()
                .Include(r => r.ReservationSeats)
                .Where(r => r.EventOccurrenceId == eventOccurrenceId)
                .ToList();

            var holds = _dbContext.seatHolds
                .AsNoTracking()
                .Where(h => h.EventOccurrenceId == eventOccurrenceId)
                .ToList();

            var eventOverrideMap = eventSeatOverrides.ToDictionary(x => x.SeatId, x => x);
            var occurrenceOverrideMap = occurrenceSeatOverrides.ToDictionary(x => x.SeatId, x => x);

            var bookedSeatIds = reservations
                .SelectMany(r => r.ReservationSeats)
                .Select(rs => rs.SeatId)
                .ToHashSet();

            var heldSeatIds = holds
                .Select(h => h.SeatId)
                .ToHashSet();

            var sectors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seatDetails = new List<SeatDetailsDto>();

            foreach (var seat in seats)
            {
                eventOverrideMap.TryGetValue(seat.Id, out var eventOverride);
                occurrenceOverrideMap.TryGetValue(seat.Id, out var occurrenceOverride);

                var effectiveSectorName =
                    occurrenceOverride?.Sector?.Name
                    ?? eventOverride?.Sector?.Name
                    ?? seat.Sector?.Name
                    ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(effectiveSectorName))
                {
                    sectors.Add(effectiveSectorName);
                }

                string status;
                if (bookedSeatIds.Contains(seat.Id))
                {
                    status = "Booked";
                }
                else if (heldSeatIds.Contains(seat.Id))
                {
                    status = "Reserved";
                }
                else
                {
                    status = "Available";
                }

                var finalPrice = ResolveFinalPrice(seat, eventOverride, occurrenceOverride);

                seatDetails.Add(new SeatDetailsDto
                {
                    seatId = seat.Id,
                    row = seat.Row,
                    column = seat.Column,
                    sector = effectiveSectorName,
                    price = finalPrice,
                    status = status
                });
            }

            return new SeatMapDto
            {
                sectors = sectors.OrderBy(x => x).ToList(),
                seats = seatDetails
            };
        }

        private static decimal ResolveFinalPrice(Seat seat, EventSeatOverride? eventOverride, OccurrenceSeatOverride? occurrenceOverride)
        {
            if (occurrenceOverride?.PriceOverride != null)
            {
                return occurrenceOverride.PriceOverride.Value;
            }

            if (eventOverride?.PriceOverride != null)
            {
                return eventOverride.PriceOverride.Value;
            }

            if (seat.PriceOverride != null)
            {
                return seat.PriceOverride.Value;
            }

            if (occurrenceOverride?.Sector?.BasePrice != null)
            {
                return occurrenceOverride.Sector.BasePrice;
            }

            if (eventOverride?.Sector?.BasePrice != null)
            {
                return eventOverride.Sector.BasePrice;
            }

            if (seat.Sector?.BasePrice != null)
            {
                return seat.Sector.BasePrice;
            }

            return 0m;
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

        public SeatMapDto GetSeatMap(string eventOccurrenceId)
        {
            EventOccurrence eventOccurrence = _dbContext.EventOccurrences.Where(e => e.Id == eventOccurrenceId).First();
            Event @event = _dbContext.EventOccurrences.Where(e => e.Id == eventOccurrenceId).First()?.Event;
            if (eventOccurrence == null)
            {
                throw new EventNotFoundException("Invalid eventOccurrenceId: " + eventOccurrenceId);
            }

            Dictionary<string, decimal> seatPrices = new Dictionary<string, decimal>();
            List<EventSeatOverride> eventSeatOverrides = _dbContext.EventSeatOverrides.Where(e => e.EventId == @event.Id).ToList();
            List<OccurrenceSeatOverride> occurrenceSeatOverrides = _dbContext.OccurrenceSeatOverrides.Where(o => o.Occurrence.Id == eventOccurrence.Id).ToList();
            HashSet<string> sectors = new HashSet<string>();

            foreach (var occurrenceSeatOverride in occurrenceSeatOverrides)
            {
                if (occurrenceSeatOverride.PriceOverride != null)
                {
                    seatPrices[occurrenceSeatOverride.SeatId] = occurrenceSeatOverride.PriceOverride.Value;
                }
            }

            foreach (var eventSeatOverride in eventSeatOverrides)
            {
                if (!sectors.Contains(eventSeatOverride.Seat.Sector.Name))
                {
                    sectors.Add(eventSeatOverride.Seat.Sector.Name);
                }

                if (seatPrices.ContainsKey(eventSeatOverride.SeatId))
                {
                    continue;
                }

                if (eventSeatOverride.PriceOverride != null)
                {
                    seatPrices.Add(eventSeatOverride.SeatId, eventSeatOverride.PriceOverride.Value);
                }
                else if (eventSeatOverride.Seat.PriceOverride != null)
                {
                    seatPrices.Add(eventSeatOverride.SeatId, eventSeatOverride.Seat.PriceOverride.Value);
                }
                else
                {
                    seatPrices.Add(eventSeatOverride.SeatId, eventSeatOverride.Seat.Sector.BasePrice);                 
                }
            }  
            
            List<SeatDetailsDto> seatDeatails = new List<SeatDetailsDto>();
            foreach (var eventSeatOverride in eventSeatOverrides)
            {
                string status;
                if (_dbContext.ReservationSeats.Count(r => r.SeatId == eventSeatOverride.SeatId) > 0)
                {
                    status = "Booked";
                }
                else if (_dbContext.seatHolds.Count(s => s.SeatId == eventSeatOverride.SeatId) > 0)
                {
                    status = "Reserved";
                }
                else
                {
                    status = "Available";
                }
                decimal finalPrice = seatPrices[eventSeatOverride.SeatId];
                SeatDetailsDto seatDetails = MapSeatDetailsDto(eventSeatOverride, finalPrice, status);
                seatDeatails.Add(seatDetails);
            }

            SeatMapDto seatMapDto = new SeatMapDto();
            seatMapDto.sectors = sectors.ToList();
            seatMapDto.seats = seatDeatails;

            return seatMapDto;
        }

        public SeatAvailabilityResponseDto getSeatAvailability(SeatAvailabilityRequestDto request)
        {
            if (_dbContext.EventOccurrences.Find(request.eventOccurrenceId) == null)
            {
                throw new EventNotFoundException($"EventOccurrence with this id_ {request.eventOccurrenceId} could not be found");
            }
            List<ReservationSeat> reservationSeats = _dbContext.ReservationSeats.Where(rs => request.seatIds.Contains(rs.SeatId)).ToList();
            SeatAvailabilityResponseDto responseDto = new SeatAvailabilityResponseDto();
            responseDto.valid = reservationSeats.Count == 0;
            responseDto.unavailableSeats = reservationSeats.Select(rs => rs.Id).ToList();
            return responseDto;
        }

        public SeatDetailsDto MapSeatDetailsDto(EventSeatOverride eventSeatOverride, decimal finalPrice, string status)
        {
            SeatDetailsDto seatDetailsDto = new SeatDetailsDto();
            seatDetailsDto.seatId = eventSeatOverride.SeatId;
            seatDetailsDto.row = eventSeatOverride.Seat.Row;
            seatDetailsDto.column = eventSeatOverride.Seat.Column;
            seatDetailsDto.sector = eventSeatOverride.Seat.Sector.Name;
            seatDetailsDto.price = finalPrice;
            seatDetailsDto.status = status;

            return seatDetailsDto;

        }
    }
}