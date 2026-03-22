using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Venue;

public class VenueUpdateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string City { get; set; } = string.Empty;
    
    [Required]
    public string PostalCode { get; set; } = string.Empty;
    
    [Required]
    public string AddressLine { get; set; } = string.Empty;
    
    [Required]
    public string OrganizerId { get; set; } = string.Empty;
}
