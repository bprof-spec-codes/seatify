using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Seat
{
    public class SeatViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string MatrixId { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
        public string? SeatLabel { get; set; }
        public string? SectorId { get; set; }
        public decimal? PriceOverride { get; set; }
        public string SeatType { get; set; } = string.Empty;
        public bool IsBookable { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
