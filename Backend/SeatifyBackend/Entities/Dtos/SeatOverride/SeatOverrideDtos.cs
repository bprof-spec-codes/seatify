namespace Entities.Dtos.SeatOverride
{
    /// <summary>
    /// Egy ülőhely effektív konfigurációja az adott kontextusban.
    /// Tartalmazza, hogy az érték honnan jön (auditorium/event/occurrence).
    /// </summary>
    public class EffectiveSeatDto
    {
        public string SeatId { get; set; } = string.Empty;
        public string MatrixId { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
        public string? SeatLabel { get; set; }

        // Effektív értékek (merge eredménye)
        public string? SectorId { get; set; }
        public string? SeatType { get; set; }
        public decimal? PriceOverride { get; set; }
        public decimal FinalPrice { get; set; }

        // Honnan jön az érték?
        public string SectorSource { get; set; } = "auditorium";      // "auditorium" | "event" | "occurrence"
        public string SeatTypeSource { get; set; } = "auditorium";
        public string PriceSource { get; set; } = "auditorium";
    }

    /// <summary>
    /// Merged seat map egy adott kontextusra (event vagy occurrence).
    /// </summary>
    public class EffectiveSeatMapDto
    {
        public string MatrixId { get; set; } = string.Empty;
        public string MatrixName { get; set; } = string.Empty;
        public int Rows { get; set; }
        public int Columns { get; set; }
        public string Context { get; set; } = string.Empty; // "auditorium" | "event" | "occurrence"
        public string? EventId { get; set; }
        public string? OccurrenceId { get; set; }
        public string? Currency { get; set; }
        public List<EffectiveSeatDto> Seats { get; set; } = new();
    }

    /// <summary>
    /// Bulk override kérés event vagy occurrence szinten.
    /// </summary>
    public class BulkSeatOverrideDto
    {
        public List<string> SeatIds { get; set; } = new();

        /// <summary>Ha null, a SectorId override törlődik (clearSector szükséges)</summary>
        public string? SectorId { get; set; }
        public bool ClearSector { get; set; } = false;

        public string? SeatType { get; set; }

        public decimal? PriceOverride { get; set; }
        public bool ClearPriceOverride { get; set; } = false;
    }

    /// <summary>
    /// Bulk override eredmény.
    /// </summary>
    public class BulkSeatOverrideResponseDto
    {
        public int UpsertedCount { get; set; }
        public List<string> AffectedSeatIds { get; set; } = new();
    }
}
