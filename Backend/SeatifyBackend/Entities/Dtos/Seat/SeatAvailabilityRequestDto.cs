using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.Seat
{
    public class SeatAvailabilityRequestDto
    {
        public string eventOccurrenceId {  get; set; }
        public List<string> seatIds { get; set; }
    }
}
