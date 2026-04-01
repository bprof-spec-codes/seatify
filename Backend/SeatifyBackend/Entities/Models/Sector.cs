using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Sector : IIdEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string AuditoriumId { get; set; } = string.Empty;
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [StringLength(30)]
        public string Color { get; set; } = "#FFFFFF";
        public decimal BasePrice { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public Auditorium Auditorium { get; set; } = null!;
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        public Sector()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
