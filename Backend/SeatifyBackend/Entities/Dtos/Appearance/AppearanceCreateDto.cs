namespace Entities.Dtos.Appearance
{
    public class AppearanceCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public string SurfaceColor { get; set; } = string.Empty;
        public string TextColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string LogoImageUrl { get; set; } = string.Empty;
        public string BannerImageUrl { get; set; } = string.Empty;
        public string ThemePreset { get; set; } = "Default (Blue)";
        public bool IsDefault { get; set; } = false;
    }
}
