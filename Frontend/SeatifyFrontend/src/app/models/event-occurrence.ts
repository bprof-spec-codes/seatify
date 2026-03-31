export interface EventOccurrenceEvent {
  id: string;
  name: string;
  description: string;
}

export interface EventOccurrenceVenue {
  id: string;
  name: string;
}

export interface EventOccurrenceAuditorium {
  id: string;
  name: string;
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
  status: string;
  event?: EventOccurrenceEvent | null;
  venue?: EventOccurrenceVenue | null;
  auditorium?: EventOccurrenceAuditorium | null;
}