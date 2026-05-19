import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { EventOccurrence } from '../models/event-occurrence';

export interface SelectedSeat {
  seatId: string;
  seatLabel: string;
  rowLabel: string;
  price: number;
}

@Injectable({
  providedIn: 'root'
})
export class PublicBookingStateService {
  private readonly bookingSessionIdStorageKey = 'seatify.bookingSessionId';

  private selectedSeatsSubject = new BehaviorSubject<SelectedSeat[]>([]);
  public selectedSeats$: Observable<SelectedSeat[]> = this.selectedSeatsSubject.asObservable();

  private eventOccurrenceSubject = new BehaviorSubject<EventOccurrence | null>(null);
  public eventOccurrence$: Observable<EventOccurrence | null> = this.eventOccurrenceSubject.asObservable();

  private bookingSessionIdSubject = new BehaviorSubject<string | null>(null);
  public bookingSessionId$: Observable<string | null> = this.bookingSessionIdSubject.asObservable();

  constructor() {
    this.bookingSessionIdSubject.next(this.readStoredBookingSessionId());
  }

  setEventOccurrence(occ: EventOccurrence): void {
    this.eventOccurrenceSubject.next(occ);
  }

  getEventOccurrence(): EventOccurrence | null {
    return this.eventOccurrenceSubject.value;
  }

  setSelectedSeats(seats: SelectedSeat[]): void {
    this.selectedSeatsSubject.next(seats);
  }

  getSelectedSeats(): SelectedSeat[] {
    return this.selectedSeatsSubject.value;
  }

  getTotalPrice(): number {
    return this.selectedSeatsSubject.value.reduce((sum, seat) => sum + seat.price, 0);
  }

  setBookingSessionId(sessionId: string | null): void {
    this.bookingSessionIdSubject.next(sessionId);
    this.persistBookingSessionId(sessionId);
  }

  getBookingSessionId(): string | null {
    return this.bookingSessionIdSubject.value;
  }

  clearState(): void {
    this.selectedSeatsSubject.next([]);
    this.eventOccurrenceSubject.next(null);
    this.bookingSessionIdSubject.next(null);
    this.persistBookingSessionId(null);
  }

  private readStoredBookingSessionId(): string | null {
    try {
      return window.sessionStorage.getItem(this.bookingSessionIdStorageKey);
    } catch {
      return null;
    }
  }

  private persistBookingSessionId(sessionId: string | null): void {
    try {
      if (sessionId) {
        window.sessionStorage.setItem(this.bookingSessionIdStorageKey, sessionId);
      } else {
        window.sessionStorage.removeItem(this.bookingSessionIdStorageKey);
      }
    } catch {
      // ignore storage failures
    }
  }
}
