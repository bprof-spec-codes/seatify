namespace Entities.Dtos.Appearance
{
    public class AppearanceViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string OrganizerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public string SurfaceColor { get; set; } = string.Empty;
        public string TextColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string LogoImageUrl { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;
        public string ThemePreset { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
