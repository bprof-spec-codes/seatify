using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Organizer
{
    public class OrganizerUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        [StringLength(30)]
        public string? BrandColor { get; set; }
    }
}
