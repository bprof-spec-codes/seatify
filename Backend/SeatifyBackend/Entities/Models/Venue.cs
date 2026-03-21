using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Venue : IIdEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public ICollection<Auditorium> Auditoriums { get; set; } = new List<Auditorium>();
        public Venue()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
