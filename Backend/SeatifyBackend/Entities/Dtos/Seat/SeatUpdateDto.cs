using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Seat
{
    public class SeatUpdateDto
    {
        [StringLength(20)]
        public string? SeatLabel { get; set; }

        public string? SectorId { get; set; }

        [Range(0, 999999)]
        public decimal? PriceOverride { get; set; }

        [Required]
        public string SeatType { get; set; } = string.Empty;
    }
}
