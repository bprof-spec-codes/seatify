using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class LayoutMatrix
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string AuditoriumId { get; set; } = string.Empty;
        public Auditorium Auditorium { get; set; } = null!;

        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 200)]
        public int Rows { get; set; }
        [Range(1, 200)]
        public int Columns { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public ICollection<Seat> Seats { get; set; } = new List<Seat>();

        public LayoutMatrix()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
