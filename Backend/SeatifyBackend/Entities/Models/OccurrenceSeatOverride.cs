using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    /// <summary>
    /// EventOccurrence-szintű felülírás: egy adott előadás-dátumhoz rendelt ülőhely felülírás.
    /// Felülírja mind az auditorium alapértelmezést, mind az event-szintű felülírást.
    /// </summary>
    public class OccurrenceSeatOverride
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string OccurrenceId { get; set; } = string.Empty;

        [Required]
        public string SeatId { get; set; } = string.Empty;

        /// <summary>Null = event override vagy auditorium alapértelmezés érvényes</summary>
        public string? SectorId { get; set; }

        /// <summary>Null = event override vagy auditorium alapértelmezés érvényes</summary>
        public SeatType? SeatType { get; set; }

        /// <summary>Null = magasabb szintű ár érvényes</summary>
        public decimal? PriceOverride { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Navigation
        public EventOccurrence Occurrence { get; set; } = null!;
        public Seat Seat { get; set; } = null!;
        public Sector? Sector { get; set; }
    }
}
