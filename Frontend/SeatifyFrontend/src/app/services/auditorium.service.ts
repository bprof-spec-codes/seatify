import { Injectable } from '@angular/core';
import { Auditorium } from '../models/auditorium';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuditoriumService {
  private venuesApiUrl = `${environment.baseApiUrl}/api/venues`;

  constructor(private http: HttpClient) {}

  getAuditoriumsByVenueId(venueId: string): Observable<Auditorium[]> {
    return this.http.get<Auditorium[]>(`${this.venuesApiUrl}/${venueId}/auditoriums`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
