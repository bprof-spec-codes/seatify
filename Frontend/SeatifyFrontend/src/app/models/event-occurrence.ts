export interface EventOccurrenceEvent {
  id: string;
  name: string;
  description: string;
  primaryColor: string;
  secondaryColor: string;
  accentColor: string;
  backgroundColor: string;
  surfaceColor: string;
  textColor: string;
  logoImageUrl: string;
  bannerImageUrl: string;
  themePreset: string;
  currency?: string;
}

export interface EventOccurrenceVenue {
  id: string;
  name: string;
}

export interface EventOccurrenceAuditorium {
  id: string;
  name: string;
  currency?: string;
}

export interface EventOccurrence {
  id: string;
  eventId: string;
  venueId: string;
  auditoriumId: string;
  startsAtUtc: string;
  endsAtUtc: string;
  bookingOpenAtUtc: string;
  bookingCloseAtUtc: string;
  doorsOpenAtUtc?: string | null;
  currencyOverride?: string;
  status: string;
  appearanceId?: string | null;
  event?: EventOccurrenceEvent | null;
  venue?: EventOccurrenceVenue | null;
  auditorium?: EventOccurrenceAuditorium | null;
}
