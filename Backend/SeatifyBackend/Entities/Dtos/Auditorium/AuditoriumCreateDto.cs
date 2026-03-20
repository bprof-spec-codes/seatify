namespace Entities.Dtos.Auditorium
{
    public class AuditoriumCreateDto
    {
        public string VenueId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
