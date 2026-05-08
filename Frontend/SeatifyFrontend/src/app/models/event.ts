export interface SeatifyEvent {
  id: string;
  organizerId: string;
  slug: string;
  name: string;
  description: string;
  status: string;
  currency?: string;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface CreateEventForm {
  name: string;
  slug: string;
  description: string;
}
