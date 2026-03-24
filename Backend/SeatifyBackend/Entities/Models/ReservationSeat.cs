using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ReservationSeat
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ReservationId { get; set; } = string.Empty;
        public string SeatId { get; set; } = string.Empty;
        public decimal FinalPrice { get; set; }
        public virtual Reservation Reservation { get; set; } = null!;

        // TODO: public virtual Seat Seat { get; set; } = null!; 
    }
}

