using Data;
using Entities.Dtos.SeatOverride;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Logic.Services
{
    public interface ISeatOverrideService
    {
        Task<EffectiveSeatMapDto?> GetEffectiveSeatMapForEventAsync(string eventId, string matrixId, CancellationToken ct);
        Task<EffectiveSeatMapDto?> GetEffectiveSeatMapForOccurrenceAsync(string occurrenceId, string matrixId, CancellationToken ct);
        Task<BulkSeatOverrideResponseDto> BulkUpsertEventOverrideAsync(string eventId, BulkSeatOverrideDto dto, CancellationToken ct);
        Task<BulkSeatOverrideResponseDto> BulkUpsertOccurrenceOverrideAsync(string occurrenceId, BulkSeatOverrideDto dto, CancellationToken ct);
    }

    public class SeatOverrideService : ISeatOverrideService
    {
        private readonly AppDbContext _ctx;

        public SeatOverrideService(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // GET: Effektív seat map event szinten
        // ──────────────────────────────────────────────────────────────────────────
        public async Task<EffectiveSeatMapDto?> GetEffectiveSeatMapForEventAsync(
            string eventId, string matrixId, CancellationToken ct)
        {
            var matrix = await _ctx.LayoutMatrices
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == matrixId, ct);

            if (matrix == null) return null;

            var eventExists = await _ctx.Events.AnyAsync(e => e.Id == eventId, ct);
            if (!eventExists) return null;

            var seats = await _ctx.Seats
                .AsNoTracking()
                .Where(s => s.MatrixId == matrixId)
                .OrderBy(s => s.Row).ThenBy(s => s.Column)
                .ToListAsync(ct);

            var seatIds = seats.Select(s => s.Id).ToList();

            // Event-szintű overrides
            var eventOverrides = await _ctx.EventSeatOverrides
                .AsNoTracking()
                .Where(eo => eo.EventId == eventId && seatIds.Contains(eo.SeatId))
                .ToListAsync(ct);

            var eventOverrideMap = eventOverrides.ToDictionary(eo => eo.SeatId);

            var effectiveSeats = seats.Select(seat =>
            {
                eventOverrideMap.TryGetValue(seat.Id, out var eo);
                return MergeEventLevel(seat, eo);
            }).ToList();

            return new EffectiveSeatMapDto
            {
                MatrixId = matrix.Id,
                MatrixName = matrix.Name,
                Rows = matrix.Rows,
                Columns = matrix.Columns,
                Context = "event",
                EventId = eventId,
                Seats = effectiveSeats
            };
        }

        // ──────────────────────────────────────────────────────────────────────────
        // GET: Effektív seat map occurrence szinten
        // ──────────────────────────────────────────────────────────────────────────
        public async Task<EffectiveSeatMapDto?> GetEffectiveSeatMapForOccurrenceAsync(
            string occurrenceId, string matrixId, CancellationToken ct)
        {
            var matrix = await _ctx.LayoutMatrices
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == matrixId, ct);

            if (matrix == null) return null;

            var occurrence = await _ctx.EventOccurrences
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == occurrenceId, ct);

            if (occurrence == null) return null;

            var seats = await _ctx.Seats
                .AsNoTracking()
                .Where(s => s.MatrixId == matrixId)
                .OrderBy(s => s.Row).ThenBy(s => s.Column)
                .ToListAsync(ct);

            var seatIds = seats.Select(s => s.Id).ToList();

            // Event-szintű overrides
            var eventOverrides = await _ctx.EventSeatOverrides
                .AsNoTracking()
                .Where(eo => eo.EventId == occurrence.EventId && seatIds.Contains(eo.SeatId))
                .ToListAsync(ct);

            // Occurrence-szintű overrides
            var occOverrides = await _ctx.OccurrenceSeatOverrides
                .AsNoTracking()
                .Where(oo => oo.OccurrenceId == occurrenceId && seatIds.Contains(oo.SeatId))
                .ToListAsync(ct);

            var eventOverrideMap = eventOverrides.ToDictionary(eo => eo.SeatId);
            var occOverrideMap = occOverrides.ToDictionary(oo => oo.SeatId);

            var effectiveSeats = seats.Select(seat =>
            {
                eventOverrideMap.TryGetValue(seat.Id, out var eo);
                occOverrideMap.TryGetValue(seat.Id, out var oo);
                return MergeOccurrenceLevel(seat, eo, oo);
            }).ToList();

            return new EffectiveSeatMapDto
            {
                MatrixId = matrix.Id,
                MatrixName = matrix.Name,
                Rows = matrix.Rows,
                Columns = matrix.Columns,
                Context = "occurrence",
                EventId = occurrence.EventId,
                OccurrenceId = occurrenceId,
                Seats = effectiveSeats
            };
        }

        // ──────────────────────────────────────────────────────────────────────────
        // PATCH: Bulk upsert event override
        // ──────────────────────────────────────────────────────────────────────────
        public async Task<BulkSeatOverrideResponseDto> BulkUpsertEventOverrideAsync(
            string eventId, BulkSeatOverrideDto dto, CancellationToken ct)
        {
            ValidateBulkDto(dto);

            var eventExists = await _ctx.Events.AnyAsync(e => e.Id == eventId, ct);
            if (!eventExists) throw new KeyNotFoundException($"Event not found: {eventId}");

            var uniqueSeatIds = dto.SeatIds.Select(s => s.Trim()).Distinct().ToList();

            var seats = await _ctx.Seats
                .Where(s => uniqueSeatIds.Contains(s.Id))
                .ToListAsync(ct);

            if (seats.Count != uniqueSeatIds.Count)
                throw new ArgumentException("Some seat IDs were not found.");

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorExists = await _ctx.Sectors.AnyAsync(s => s.Id == dto.SectorId, ct);
                if (!sectorExists) throw new ArgumentException($"Sector not found: {dto.SectorId}");
            }

            SeatType? parsedSeatType = ParseSeatType(dto.SeatType);

            // Meglévő overrides betöltése
            var existing = await _ctx.EventSeatOverrides
                .Where(eo => eo.EventId == eventId && uniqueSeatIds.Contains(eo.SeatId))
                .ToListAsync(ct);

            var existingMap = existing.ToDictionary(eo => eo.SeatId);
            var now = DateTime.UtcNow;
            var affected = new List<string>();

            foreach (var seat in seats)
            {
                if (!existingMap.TryGetValue(seat.Id, out var existing_override))
                {
                    // INSERT
                    var newOverride = new EventSeatOverride
                    {
                        EventId = eventId,
                        SeatId = seat.Id,
                        SectorId = dto.ClearSector ? null : dto.SectorId,
                        SeatType = parsedSeatType,
                        PriceOverride = dto.ClearPriceOverride ? null : dto.PriceOverride,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    };
                    _ctx.EventSeatOverrides.Add(newOverride);
                }
                else
                {
                    // UPDATE
                    if (!string.IsNullOrWhiteSpace(dto.SectorId))
                        existing_override.SectorId = dto.SectorId;
                    else if (dto.ClearSector)
                        existing_override.SectorId = null;

                    if (parsedSeatType.HasValue)
                        existing_override.SeatType = parsedSeatType;

                    if (dto.PriceOverride.HasValue)
                        existing_override.PriceOverride = dto.PriceOverride;
                    else if (dto.ClearPriceOverride)
                        existing_override.PriceOverride = null;

                    existing_override.UpdatedAtUtc = now;
                }

                affected.Add(seat.Id);
            }

            await _ctx.SaveChangesAsync(ct);

            return new BulkSeatOverrideResponseDto
            {
                UpsertedCount = affected.Count,
                AffectedSeatIds = affected
            };
        }

        // ──────────────────────────────────────────────────────────────────────────
        // PATCH: Bulk upsert occurrence override
        // ──────────────────────────────────────────────────────────────────────────
        public async Task<BulkSeatOverrideResponseDto> BulkUpsertOccurrenceOverrideAsync(
            string occurrenceId, BulkSeatOverrideDto dto, CancellationToken ct)
        {
            ValidateBulkDto(dto);

            var occurrenceExists = await _ctx.EventOccurrences.AnyAsync(o => o.Id == occurrenceId, ct);
            if (!occurrenceExists) throw new KeyNotFoundException($"Occurrence not found: {occurrenceId}");

            var uniqueSeatIds = dto.SeatIds.Select(s => s.Trim()).Distinct().ToList();

            var seats = await _ctx.Seats
                .Where(s => uniqueSeatIds.Contains(s.Id))
                .ToListAsync(ct);

            if (seats.Count != uniqueSeatIds.Count)
                throw new ArgumentException("Some seat IDs were not found.");

            if (!string.IsNullOrWhiteSpace(dto.SectorId))
            {
                var sectorExists = await _ctx.Sectors.AnyAsync(s => s.Id == dto.SectorId, ct);
                if (!sectorExists) throw new ArgumentException($"Sector not found: {dto.SectorId}");
            }

            SeatType? parsedSeatType = ParseSeatType(dto.SeatType);

            var existing = await _ctx.OccurrenceSeatOverrides
                .Where(oo => oo.OccurrenceId == occurrenceId && uniqueSeatIds.Contains(oo.SeatId))
                .ToListAsync(ct);

            var existingMap = existing.ToDictionary(oo => oo.SeatId);
            var now = DateTime.UtcNow;
            var affected = new List<string>();

            foreach (var seat in seats)
            {
                if (!existingMap.TryGetValue(seat.Id, out var existing_override))
                {
                    var newOverride = new OccurrenceSeatOverride
                    {
                        OccurrenceId = occurrenceId,
                        SeatId = seat.Id,
                        SectorId = dto.ClearSector ? null : dto.SectorId,
                        SeatType = parsedSeatType,
                        PriceOverride = dto.ClearPriceOverride ? null : dto.PriceOverride,
                        CreatedAtUtc = now,
                        UpdatedAtUtc = now
                    };
                    _ctx.OccurrenceSeatOverrides.Add(newOverride);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.SectorId))
                        existing_override.SectorId = dto.SectorId;
                    else if (dto.ClearSector)
                        existing_override.SectorId = null;

                    if (parsedSeatType.HasValue)
                        existing_override.SeatType = parsedSeatType;

                    if (dto.PriceOverride.HasValue)
                        existing_override.PriceOverride = dto.PriceOverride;
                    else if (dto.ClearPriceOverride)
                        existing_override.PriceOverride = null;

                    existing_override.UpdatedAtUtc = now;
                }

                affected.Add(seat.Id);
            }

            await _ctx.SaveChangesAsync(ct);

            return new BulkSeatOverrideResponseDto
            {
                UpsertedCount = affected.Count,
                AffectedSeatIds = affected
            };
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Privát segédmetódusok
        // ──────────────────────────────────────────────────────────────────────────

        private static EffectiveSeatDto MergeEventLevel(Seat seat, EventSeatOverride? eo)
        {
            return new EffectiveSeatDto
            {
                SeatId = seat.Id,
                MatrixId = seat.MatrixId,
                Row = seat.Row,
                Column = seat.Column,
                SeatLabel = seat.SeatLabel,
                SectorId = eo?.SectorId ?? seat.SectorId,
                SeatType = (eo?.SeatType ?? seat.SeatType).ToString(),
                PriceOverride = eo?.PriceOverride ?? seat.PriceOverride,
                SectorSource = eo?.SectorId != null ? "event" : "auditorium",
                SeatTypeSource = eo?.SeatType != null ? "event" : "auditorium",
                PriceSource = eo?.PriceOverride != null ? "event" : "auditorium"
            };
        }

        private static EffectiveSeatDto MergeOccurrenceLevel(
            Seat seat, EventSeatOverride? eo, OccurrenceSeatOverride? oo)
        {
            var effectiveSectorId = oo?.SectorId ?? eo?.SectorId ?? seat.SectorId;
            var effectiveSeatType = (oo?.SeatType ?? eo?.SeatType ?? seat.SeatType).ToString();
            var effectivePrice = oo?.PriceOverride ?? eo?.PriceOverride ?? seat.PriceOverride;

            return new EffectiveSeatDto
            {
                SeatId = seat.Id,
                MatrixId = seat.MatrixId,
                Row = seat.Row,
                Column = seat.Column,
                SeatLabel = seat.SeatLabel,
                SectorId = effectiveSectorId,
                SeatType = effectiveSeatType,
                PriceOverride = effectivePrice,
                SectorSource = oo?.SectorId != null ? "occurrence" : (eo?.SectorId != null ? "event" : "auditorium"),
                SeatTypeSource = oo?.SeatType != null ? "occurrence" : (eo?.SeatType != null ? "event" : "auditorium"),
                PriceSource = oo?.PriceOverride != null ? "occurrence" : (eo?.PriceOverride != null ? "event" : "auditorium")
            };
        }

        private static void ValidateBulkDto(BulkSeatOverrideDto dto)
        {
            if (dto == null || dto.SeatIds == null || !dto.SeatIds.Any())
                throw new ArgumentException("At least one SeatId must be provided.");
        }

        private static SeatType? ParseSeatType(string? seatType)
        {
            if (string.IsNullOrWhiteSpace(seatType)) return null;
            if (Enum.TryParse<SeatType>(seatType.Trim(), true, out var parsed)) return parsed;
            throw new ArgumentException($"Invalid SeatType: {seatType}");
        }
    }
}
