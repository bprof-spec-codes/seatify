export interface EventCardOccurrence {
  id: string;
  auditoriumId: string;
  startsAtUtc: string;
  endsAtUtc: string;
  status: string;
}

export interface EventCard {
  id: string;
  slug: string;
  title: string;
  description: string;
  status: string;
  venueName: string;
  auditoriumName: string;
  occurrences: EventCardOccurrence[];
}
