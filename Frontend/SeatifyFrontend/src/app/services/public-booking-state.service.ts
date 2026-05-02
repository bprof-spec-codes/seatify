import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { EventOccurrence } from '../models/event-occurrence';

export interface SelectedSeat {
  seatId: string;
  seatLabel: string;
  price: number;
}

@Injectable({
  providedIn: 'root'
})
export class PublicBookingStateService {
  private selectedSeatsSubject = new BehaviorSubject<SelectedSeat[]>([]);
  public selectedSeats$: Observable<SelectedSeat[]> = this.selectedSeatsSubject.asObservable();

  private eventOccurrenceSubject = new BehaviorSubject<EventOccurrence | null>(null);
  public eventOccurrence$: Observable<EventOccurrence | null> = this.eventOccurrenceSubject.asObservable();

  constructor() { }

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

  clearState(): void {
    this.selectedSeatsSubject.next([]);
    this.eventOccurrenceSubject.next(null);
  }
}
