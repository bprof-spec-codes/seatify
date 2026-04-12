import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from 'rxjs';
import { SeatMap } from '../models/seat-map';
import { environment } from '../../environments/environment';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BulkSeatUpdateDto, Seat, UpdateSeatDto } from '../models/seat';

@Injectable({
  providedIn: 'root'
})
export class SeatService {
  private apiUrl = `${environment.baseApiUrl}/api`
  private seatMapSource = new BehaviorSubject<SeatMap | null>(null)
  seatMap$ = this.seatMapSource.asObservable()

  private seatSource = new BehaviorSubject<Seat | null>(null)
  seat$ = this.seatSource.asObservable()


  constructor(private http: HttpClient) { }

  getSeatMapByMatrixId(matrixId: string): Observable<SeatMap> {
    return this.http.get<SeatMap>(`${this.apiUrl}/layout-matrices/${matrixId}/seat-map`).pipe(
      map(seatMap => this.mapSeatMapDates(seatMap)),
      tap(seatMap => this.seatMapSource.next(seatMap)),
      catchError(this.handleError)
    )
  }

  updateSeat(seatId: string, dto: UpdateSeatDto): Observable<Seat> {
    return this.http.put<Seat>(`${this.apiUrl}/seats/${seatId}`, dto).pipe(
      map(updatedSeat => this.mapSeatDates(updatedSeat)),
      tap(updatedSeat =>{
        const currentSeat = this.seatSource.getValue();
        if (currentSeat && currentSeat.id === seatId) {
          this.seatSource.next(updatedSeat);
        }
      }),
      catchError(this.handleError)
    )
  }

  bulkUpdateSeats(dto: BulkSeatUpdateDto): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/seats/bulk`, dto).pipe(
      catchError(this.handleError)
    )
  }


  clearSeatMap(): void {
    this.seatMapSource.next(null);
  }

  setSeatMap(seatMap: SeatMap | null): void {
    this.seatMapSource.next(seatMap);
  }

  private mapSeatMapDates(seatMap: SeatMap): SeatMap {
    return {
      ...seatMap,
      seats: seatMap.seats.map(seat => this.mapSeatDates(seat))
    };
  }

  private mapSeatDates(seat: Seat): Seat {
    return {
      ...seat,
      createdAtUtc: new Date(seat.createdAtUtc),
      updatedAtUtc: new Date(seat.updatedAtUtc)
    };
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
