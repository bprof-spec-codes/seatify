using System.ComponentModel.DataAnnotations;
using Entities.Helpers;

namespace Entities.Models
{
    public class Venue : IIdEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string OrganizerId { get; set; } = string.Empty; // TODO: FK
        
        public Venue()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
