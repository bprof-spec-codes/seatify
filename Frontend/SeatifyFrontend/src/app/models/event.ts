export interface SeatifyEvent {
  id: string;
  organizerId: string;
  slug: string;
  name: string;
  description: string;
  status: string;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface CreateEventForm {
  name: string;
  slug: string;
  description: string;
}
