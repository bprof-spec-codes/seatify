using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    /// <summary>
    /// Event-szintű felülírás: egy adott eseményhez rendelt ülőhely felülírás.
    /// Felülírja az auditorium alapértelmezést az eseményre vonatkozóan.
    /// </summary>
    public class EventSeatOverride
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string EventId { get; set; } = string.Empty;

        [Required]
        public string SeatId { get; set; } = string.Empty;

        /// <summary>Null = auditorium alapértelmezés érvényes</summary>
        public string? SectorId { get; set; }

        /// <summary>Null = auditorium alapértelmezés érvényes</summary>
        public SeatType? SeatType { get; set; }

        /// <summary>Null = sector alapár érvényes</summary>
        public decimal? PriceOverride { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Navigation
        public Event Event { get; set; } = null!;
        public Seat Seat { get; set; } = null!;
        public Sector? Sector { get; set; }
    }
}
