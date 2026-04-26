namespace Entities.Dtos.Auditorium
{
    public class AuditoriumCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Currency { get; set; } = "HUF";
    }
}
