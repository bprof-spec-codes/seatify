using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Seat
{
    public class BulkSeatUpdateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one SeatId must be provided.")]
        public List<string> SeatIds { get; set; } = new List<string>();

        public string? SectorId { get; set; }

        public string? SeatType { get; set; }

        [Range(0, 999999)]
        public decimal? PriceOverride { get; set; }

        public bool ClearSector { get; set; }
        public bool ClearPriceOverride { get; set; }
    }
}
