using Entities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Organizer : IIdEntity
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(30)]
        public string BrandColor { get; set; } = "#FFFFFF";

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        public virtual ICollection<Venue> Venues { get; set; } = new List<Venue>();

        public Organizer()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
