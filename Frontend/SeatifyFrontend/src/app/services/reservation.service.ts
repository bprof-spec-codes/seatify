import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface BookingCheckoutRequest {
  eventOccurrenceId: string;
  seatIds: string[];
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  bookingSessionId?: string;
}

export interface TicketItem {
  ticketId: string;
  seatId: string;
  seatLabel: string;
  qrCodeBase64: string;
  price: number;
}

export interface BookingCheckoutResponse {
  bookingId: string;
  eventId: string;
  tickets: TicketItem[];
  totalPrice: number;
  currency: string;
  qrCodeBase64: string;
  pdfBase64: string;
}

export interface ReservationSeatView {
  seatId: string;
  finalPrice: number;
}

export interface ReservationView {
  id: string;
  customerName: string;
  customerEmail: string;
  status: string;
  createdAtUtc: string;
  reservedSeats: ReservationSeatView[];
}

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  private apiUrl = `${environment.baseApiUrl}/api`;

  constructor(private http: HttpClient) { }

  checkoutReservation(request: BookingCheckoutRequest): Observable<BookingCheckoutResponse> {
    return this.http.post<BookingCheckoutResponse>(`${this.apiUrl}/bookings/checkout`, request);
  }

  getReservationsForOccurrence(occurrenceId: string): Observable<ReservationView[]> {
    return this.http.get<ReservationView[]>(`${this.apiUrl}/by-event-occurrences/${occurrenceId}/reservations`);
  }
}
