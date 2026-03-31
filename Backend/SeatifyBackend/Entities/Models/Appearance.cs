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

        public string PrimaryColor { get; set; } = string.Empty;
        
        public string SecondaryColor { get; set; } = string.Empty;
        
        public string LogoImageUrl { get; set; } = string.Empty;
        
        public string BannerImageUrl { get; set; } = string.Empty;
        
        public string ThemePreset { get; set; } = "Default (Blue)";

        public bool IsDefault { get; set; } = false;

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
