using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.SeatMap
{
    public class SeatDetailsDto
    {
        public string seatId {  get; set; }
        public int row { get; set;}
        public int column { get; set;}
        public decimal price { get; set;}
        public string status { get; set;}
        public string sector { get; set;}
    }
}
