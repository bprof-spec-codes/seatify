using Entities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{

    public enum SeatType
    {
        Seat = 0,
        AccessibleSeat = 1,
        Aisle = 2
    }
    public class Seat : IIdEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string MatrixId { get; set; } = string.Empty;

        public int Row { get; set; }
        public int Column { get; set; }

        [StringLength(20)]
        public string? SeatLabel { get; set; }

        public string? SectorId { get; set; }

        public decimal? PriceOverride { get; set; }

        public SeatType SeatType { get; set; } = SeatType.Seat;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public LayoutMatrix LayoutMatrix { get; set; } = null!;
        public Sector? Sector { get; set; }

        public Seat()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
