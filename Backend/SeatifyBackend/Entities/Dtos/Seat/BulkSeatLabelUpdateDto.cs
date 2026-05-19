using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Seat
{
    public class BulkSeatLabelUpdateDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one seat label item must be provided.")]
        public List<BulkSeatLabelUpdateItemDto> Items { get; set; } = new();
    }

    public class BulkSeatLabelUpdateItemDto
    {
        [Required]
        public string SeatId { get; set; } = string.Empty;

        public string? SeatLabel { get; set; }
    }
}
