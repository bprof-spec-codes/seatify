using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

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

        public string OrganizerId { get; set; } = string.Empty;

        public virtual Organizer? Organizer { get; set; }
        public ICollection<Auditorium> Auditoriums { get; set; } = new List<Auditorium>();

        public virtual ICollection<EventOccurrence> EventOccurrences { get; set; } = new List<EventOccurrence>();
    }
}
