import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BookingSession } from '../models/booking.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class BookingSessionService {
  private apiUrl = `${environment.baseApiUrl}/api/public/booking-sessions`;

  private readonly bookingPath = '/api/bookings';

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) { }

  private api(path: string): string {
    return `${this.configService.cfg.baseApiUrl}${path}`;
  }

  getActiveSession(sessionId: string): Observable<BookingSession> {
    return this.http.get<BookingSession>(`${this.apiUrl}/${sessionId}`);
  }

  checkout(sessionId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${sessionId}/checkout`, {});
  }

  createBookingSession(eventOccurrenceId: string): Observable<BookingSession> {
    return this.http.post<BookingSession>(this.apiUrl, { eventOccurrenceId });
  }

  holdSeat(sessionId: string, seatId: string): Observable<BookingSession> {
    return this.http.post<BookingSession>(`${this.apiUrl}/${sessionId}/holds`, { seatId });
  }

  releaseSeat(sessionId: string, seatId: string): Observable<BookingSession> {
    return this.http.delete<BookingSession>(`${this.apiUrl}/${sessionId}/holds/${seatId}`);
  }
}
