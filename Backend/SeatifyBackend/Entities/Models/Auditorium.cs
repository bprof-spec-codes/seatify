using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Auditorium : IIdEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string VenueId { get; set; } = string.Empty;
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public Venue Venue { get; set; } = null!;
        public ICollection<LayoutMatrix> LayoutMatrices { get; set; } = new List<LayoutMatrix>();

        public virtual List<EventOccurrence> EventOccurrences { get; set; } = new();
        public Auditorium()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
