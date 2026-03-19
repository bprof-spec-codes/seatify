using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Auditorium;

public class AuditoriumViewDto
{
    [Required]
    public string Id { get; set; }
    
    [Required]
    public string VenueId { get; set; }
}
