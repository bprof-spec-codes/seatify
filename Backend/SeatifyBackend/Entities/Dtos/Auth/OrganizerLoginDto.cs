using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Auth
{
    public class OrganizerLoginDto
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
