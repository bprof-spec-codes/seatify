using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class BookingSession
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Status { get; set; }
    }
}
