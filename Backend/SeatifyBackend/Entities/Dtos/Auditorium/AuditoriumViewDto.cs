using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Auditorium;

public class AuditoriumViewDto
{
    [Required]
    public string Id { get; set; }
    
    [Required]
    public string VenueId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public List<LayoutMatrix> LayoutMatrices { get; set; } = new(); //todo: LayoutMatrix helyett LayoutMatrixViewDto-ra cserélni!!

    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
}
