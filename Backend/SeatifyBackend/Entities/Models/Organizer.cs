using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Organizer : IIdEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public ICollection<Appearance> Appearances { get; set; } = new List<Appearance>();
        public ICollection<Venue> Venues { get; set; } = new List<Venue>();

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        public Organizer()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
