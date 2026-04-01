import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { ReservationRequest, Reservation } from '../models/booking.model';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {

  constructor() { }

  // Mock method to create reservation
  createReservation(request: ReservationRequest): Observable<Reservation> {
    const mockReservation: Reservation = {
      id: 'res_' + Math.random().toString(36).substring(7),
      status: 'Confirmed'
    };
    return of(mockReservation).pipe(delay(1000));
  }
}
