namespace Entities.Models
{
    public class Venue
    {
        public string Id { get; set; }
        string.Empty;
        public string Name { get; set; } = string.Empty;
        public Venue()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
