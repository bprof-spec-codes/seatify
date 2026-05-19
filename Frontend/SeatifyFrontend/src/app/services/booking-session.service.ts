import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BookingSession } from '../models/booking.model';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class BookingSessionService {
  private readonly bookingPath = '/api/bookings';

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) { }

  private api(path: string): string {
    return `${this.configService.cfg.baseApiUrl}${path}`;
  }

  private get bookingSessionsUrl(): string {
    return `${this.configService.apiBaseUrl}/api/public/booking-sessions`;
  }

  getActiveSession(sessionId: string): Observable<BookingSession> {
    return this.http.get<BookingSession>(`${this.bookingSessionsUrl}/${sessionId}`);
  }

  checkout(sessionId: string): Observable<void> {
    return this.http.post<void>(`${this.bookingSessionsUrl}/${sessionId}/checkout`, {});
  }

  createBookingSession(eventOccurrenceId: string): Observable<BookingSession> {
    return this.http.post<BookingSession>(this.bookingSessionsUrl, { eventOccurrenceId });
  }

  holdSeat(sessionId: string, seatId: string): Observable<BookingSession> {
    return this.http.post<BookingSession>(`${this.bookingSessionsUrl}/${sessionId}/holds`, { seatId });
  }

  releaseSeat(sessionId: string, seatId: string): Observable<BookingSession> {
    return this.http.delete<BookingSession>(`${this.bookingSessionsUrl}/${sessionId}/holds/${seatId}`);
  }
}
