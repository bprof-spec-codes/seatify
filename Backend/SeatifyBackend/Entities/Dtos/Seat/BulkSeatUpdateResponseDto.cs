using System.Collections.Generic;

namespace Entities.Dtos.Seat
{
    public class BulkSeatUpdateResponseDto
    {
        public int UpdatedCount { get; set; }
        public List<string> UpdatedSeatIds { get; set; } = new List<string>();
    }
}
