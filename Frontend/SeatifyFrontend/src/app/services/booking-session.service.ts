import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { BookingSession } from '../models/booking.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BookingSessionService {
  private apiUrl = `${environment.baseApiUrl}/api/bookings`;

  constructor(private http: HttpClient) { }

  // Mock method to get the current booking session for checkout
  getActiveSession(sessionId: string): Observable<BookingSession> {
    const mockSession: BookingSession = {
      id: sessionId,
      eventOccurrenceId: 'occ_001',
      phase: 'Checkout',
      status: 'Active',
      expiresAtUtc: new Date(Date.now() + 10 * 60000).toISOString(),
      holds: [
        { id: 'hold_1', seatId: 'seat_A_12', rowLabel: 'A', seatLabel: '12', basePrice: 6500 },
        { id: 'hold_2', seatId: 'seat_A_13', rowLabel: 'A', seatLabel: '13', basePrice: 6500 }
      ]
    };
    return of(mockSession).pipe(delay(500));
  }

  // Mock method to transition session to checkout
  checkout(sessionId: string): Observable<void> {
    return of(void 0).pipe(delay(500));
  }

  createBookingSession()
  {
    // mock: { status: "asd" } <- null-al nem működik
    return this.http.post(`${this.apiUrl}/checkout`, { status: "asd" }, {
      responseType: 'blob',
      headers: new HttpHeaders({ 'Accept': 'image/png' })
    });
  }
}
