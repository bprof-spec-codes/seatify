using System.ComponentModel.DataAnnotations;
using Entities.Helpers;

namespace Entities.Models
{
    public class Appearance : IIdEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string OrganizerId { get; set; } = string.Empty;

        public Organizer Organizer { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
