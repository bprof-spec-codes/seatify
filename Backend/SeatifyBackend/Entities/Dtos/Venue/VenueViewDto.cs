using System.ComponentModel.DataAnnotations;
using Entities.Dtos.Auditorium;
using Entities.Models;

namespace Entities.Dtos.Venue;

public class VenueViewDto
{
    [Required]
    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string City { get; set; } = string.Empty;
    
    [Required]
    public string PostalCode { get; set; } = string.Empty;
    
    [Required]
    public string AddressLine { get; set; } = string.Empty;
    
    public ICollection<AuditoriumViewDto> Auditoriums { get; set; } = new List<AuditoriumViewDto>();
}
