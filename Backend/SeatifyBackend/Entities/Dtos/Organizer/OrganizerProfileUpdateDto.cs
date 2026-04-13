using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Organizer
{
    public class OrganizerProfileUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
