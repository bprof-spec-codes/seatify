using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class BookingSession
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string EventOccurrendeId { get; set; }
        public string Phase { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public virtual EventOccurrence EventOccurrence { get; set; }
    }
}
