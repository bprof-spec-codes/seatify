using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class LayoutMatrix
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string AuditoriumId { get; set; } = string.Empty;
        public Auditorium Auditorium { get; set; } = null!;

        public LayoutMatrix()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
