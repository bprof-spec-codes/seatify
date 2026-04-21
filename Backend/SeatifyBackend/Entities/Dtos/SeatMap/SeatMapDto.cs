using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.SeatMap
{
    public class SeatMapDto
    {
        public List<string> sectors { get; set; }
        public List<SeatDetailsDto> seats { get; set; }
    }
}
