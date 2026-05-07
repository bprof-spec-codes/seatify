export interface EventOccurrenceEvent {
  id: string;
  name: string;
  description: string;
  primaryColor?: string;
  secondaryColor?: string;
  logoImageUrl?: string;
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
  effectiveCurrency: string;
  event?: EventOccurrenceEvent | null;
  venue?: EventOccurrenceVenue | null;
  auditorium?: EventOccurrenceAuditorium | null;
}
