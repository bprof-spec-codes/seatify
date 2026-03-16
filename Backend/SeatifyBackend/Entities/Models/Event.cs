namespace Entities.Models
{
    public class Event
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public decimal BasePrice { get; set; }

        public Event()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
