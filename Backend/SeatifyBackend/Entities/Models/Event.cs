using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Event
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrganizerId { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Published";
        public string? Currency { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual List<EventOccurrence> EventOccurrences { get; set; } = new();
        
        public string? AppearanceId { get; set; }
        public virtual Appearance? Appearance { get; set; }
    }
}
