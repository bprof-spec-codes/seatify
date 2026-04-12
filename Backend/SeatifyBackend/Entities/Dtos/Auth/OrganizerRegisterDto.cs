using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos.Auth
{
    public class OrganizerRegisterDto
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]

        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
