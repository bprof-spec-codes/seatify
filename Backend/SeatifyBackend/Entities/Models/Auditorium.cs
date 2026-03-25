using System.ComponentModel.DataAnnotations;
using Entities.Helpers;

namespace Entities.Models;

public class Auditorium : IIdEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public string VenueId { get; set; } = string.Empty;
    
    public virtual Venue? Venue { get; set; }
    
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public ICollection<LayoutMatrix> LayoutMatrices { get; set; } = new List<LayoutMatrix>();
    
    public ICollection<Sector> Sectors { get; set; } = new List<Sector>();

    public virtual ICollection<EventOccurrence> EventOccurrences { get; set; } = new List<EventOccurrence>();
}
