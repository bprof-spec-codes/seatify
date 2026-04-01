using System.ComponentModel.DataAnnotations;
using Entities.Helpers;

namespace Entities.Models
{
    public class EventAppearance : IIdEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string EventId { get; set; } = string.Empty;
        public Event Event { get; set; } = null!;

        public string PrimaryColor { get; set; } = string.Empty;
        
        public string SecondaryColor { get; set; } = string.Empty;
        
        public string LogoImageUrl { get; set; } = string.Empty;
        
        public string BannerImageUrl { get; set; } = string.Empty;
        
        public string ThemePreset { get; set; } = "Default (Blue)";

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
