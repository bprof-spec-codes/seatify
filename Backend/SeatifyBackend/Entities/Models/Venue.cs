using System.ComponentModel.DataAnnotations;
using Entities.Helpers;

namespace Entities.Models
{
    public class Venue : IIdEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Name { get; set; } = string.Empty;
        
        public string City { get; set; } = string.Empty;
        
        public string PostalCode { get; set; } = string.Empty;
        
        public string AddressLine { get; set; } = string.Empty;
        
        // TODO: FK
        public string OrganizerId { get; set; } = string.Empty;
        
        public ICollection<Auditorium> Auditoriums { get; set; } = new List<Auditorium>();
    }
}
