using Entities.Dtos.Venue;

namespace Entities.Dtos.Organizer
{
    public class OrganizerViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<VenueViewDto> Venues { get; set; } = new List<VenueViewDto>();
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
