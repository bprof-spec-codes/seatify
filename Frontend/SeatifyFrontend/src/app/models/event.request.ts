export default interface EventRequest{
  name: string;
  description: string;
  startsAt: Date;
  endsAt: Date;
  basePrice: number;
  //new fields bellow:
  logoImageUrl?: string;
  bannerImageUrl?: string;
  themePreset?: string;
  venueId?: string;
  auditoriumId?: string;

}