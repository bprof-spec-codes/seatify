using System.ComponentModel.DataAnnotations;
using Entities.Helpers;

namespace Entities.Models;

public class Auditorium : IIdEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string VenueId { get; set; } = string.Empty;
    
    public virtual Venue? Venue { get; set; }
}
