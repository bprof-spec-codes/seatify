namespace Entities.Models
{
    public class Event
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string OrganizerId { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Published";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public virtual List<EventOccurrence> EventOccurrences { get; set; } = new();
    }
}
