export interface Appearance {
  id: string;
  organizerId: string;
  name: string;
  primaryColor: string;
  accentColor: string;
  backgroundColor: string;
  surfaceColor: string;
  textColor: string;
  secondaryColor: string;
  logoImageUrl: string;
  bannerImageUrl: string;
  themePreset: string;
  fontFamily: string;
  isDefault: boolean;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface AppearanceCreateRequest {
  name: string;
  primaryColor: string;
  accentColor: string;
  backgroundColor: string;
  surfaceColor: string;
  textColor: string;
  secondaryColor: string;
  logoImageUrl: string;
  bannerImageUrl: string;
  themePreset: string;
  fontFamily: string;
  isDefault: boolean;
}
