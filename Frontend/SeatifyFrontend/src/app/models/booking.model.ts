export interface SeatHold {
  id: string;
  seatId: string;
  rowLabel: string;
  seatLabel: string;
  basePrice: number;
}

export interface BookingSession {
  id: string;
  eventOccurrenceId: string;
  phase: 'Selection' | 'Checkout' | 'Completed' | 'Expired' | 'Cancelled';
  status: 'Active' | 'Completed' | 'Expired' | 'Cancelled';
  expiresAtUtc: string;
  holds: SeatHold[];
}

export interface ReservationRequest {
  bookingSessionId: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
}

export interface Reservation {
  id: string;
  status: 'Confirmed' | 'Cancelled';
}
