namespace Entities.Dtos.Sector
{
    public class SectorCreateUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public decimal BasePrice { get; set; }
    }
}
