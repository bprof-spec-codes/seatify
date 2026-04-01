using Entities.Dtos.LayoutMatrix;
using Entities.Dtos.Sector;

namespace Entities.Dtos.Auditorium;

public class AuditoriumViewDto
{
    public string Id { get; set; } = string.Empty;

    public string VenueId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public List<LayoutMatrixViewDto> LayoutMatrices { get; set; } = new();

    public List<SectorViewDto> Sectors { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
