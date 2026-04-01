using Entities.Dtos.SeatOverride;
using Logic.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class SeatOverrideController : ControllerBase
    {
        private readonly ISeatOverrideService _seatOverrideService;

        public SeatOverrideController(ISeatOverrideService seatOverrideService)
        {
            _seatOverrideService = seatOverrideService;
        }

        // ──────────────────────────────────────────────────────────────────────────
        // EVENT szintű endpointok
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Visszaadja egy event kontextusában az effektív seat map-et (merged nézettel):
        /// OccurrenceOverride ?? EventOverride ?? AuditoriumDefault
        /// </summary>
        [HttpGet("api/events/{eventId}/seat-map/{matrixId}")]
        public async Task<IActionResult> GetEventSeatMap(
            string eventId, string matrixId, CancellationToken ct)
        {
            var result = await _seatOverrideService
                .GetEffectiveSeatMapForEventAsync(eventId, matrixId, ct);

            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Bulk upsert event-szintű seat override.
        /// Ha már létezik override az adott (eventId, seatId) párhoz, frissíti; ha nem, létrehozza.
        /// </summary>
        [HttpPatch("api/events/{eventId}/seats/bulk")]
        public async Task<IActionResult> BulkUpsertEventOverride(
            string eventId, [FromBody] BulkSeatOverrideDto dto, CancellationToken ct)
        {
            var result = await _seatOverrideService
                .BulkUpsertEventOverrideAsync(eventId, dto, ct);

            return Ok(result);
        }

        // ──────────────────────────────────────────────────────────────────────────
        // OCCURRENCE szintű endpointok
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Visszaadja egy occurrence kontextusában az effektív seat map-et (teljes merge):
        /// OccurrenceOverride ?? EventOverride ?? AuditoriumDefault
        /// </summary>
        [HttpGet("api/event-occurrences/{occurrenceId}/seat-map/{matrixId}")]
        public async Task<IActionResult> GetOccurrenceSeatMap(
            string occurrenceId, string matrixId, CancellationToken ct)
        {
            var result = await _seatOverrideService
                .GetEffectiveSeatMapForOccurrenceAsync(occurrenceId, matrixId, ct);

            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Bulk upsert occurrence-szintű seat override.
        /// A legmagasabb prioritású szintű felülírás.
        /// </summary>
        [HttpPatch("api/event-occurrences/{occurrenceId}/seats/bulk")]
        public async Task<IActionResult> BulkUpsertOccurrenceOverride(
            string occurrenceId, [FromBody] BulkSeatOverrideDto dto, CancellationToken ct)
        {
            var result = await _seatOverrideService
                .BulkUpsertOccurrenceOverrideAsync(occurrenceId, dto, ct);

            return Ok(result);
        }
    }
}
