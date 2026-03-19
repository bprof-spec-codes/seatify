namespace Entities.Dtos.Sector
{
    public class SectorViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string AuditoriumId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
    }
}
