import { Injectable } from '@angular/core';
import { Auditorium } from '../models/auditorium';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuditoriumService {
  private apiUrl = `${environment.baseApiUrl}/api/auditoriums`;
  private venuesApiUrl = `${environment.baseApiUrl}/api/venues`;
  private auditoriumsSource$ = new BehaviorSubject<Auditorium[]>([]);
  auditoriums$ = this.auditoriumsSource$.asObservable();

  constructor(private http: HttpClient) {}

  getAuditoriumsByVenueId(venueId: string): Observable<Auditorium[]> {
    return this.http.get<Auditorium[]>(`${this.venuesApiUrl}/${venueId}/auditoriums`).pipe(
      catchError(this.handleError)
    );
  }

  deleteAuditoriumById(auditoriumId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${auditoriumId}`).pipe(
      tap(() => {
        const currentAuditoriums = this.auditoriumsSource$.getValue();
        const updatedAuditoriums = currentAuditoriums.filter(a => a.id !== auditoriumId);
        this.auditoriumsSource$.next(updatedAuditoriums);
      }),
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
