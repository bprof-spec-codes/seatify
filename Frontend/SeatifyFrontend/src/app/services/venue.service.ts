import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, throwError } from 'rxjs';
import { Venue } from '../models/venue';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class VenueService {
  private apiUrl = `${environment.baseApiUrl}/api/venues`;

  constructor(private http: HttpClient) {}

  getVenue(venueId: string): Observable<Venue> {
    return this.http.get<Venue>(`${this.apiUrl}/${venueId}`).pipe(
      catchError(this.handleError)
    );
  }

  getVenuesByOrganizerId(organizerId: string): Observable<Venue[]> {
    return this.http.get<Venue[]>(`${this.apiUrl}/organizers/${organizerId}`).pipe(
      catchError(this.handleError)
    );
  }

  getVenues(): Observable<Venue[]> {
    return this.http.get<Venue[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  postVenue(venue: Venue): Observable<Venue> {
    return this.http.post<Venue>(this.apiUrl, venue).pipe(
      catchError(this.handleError)
    );
  }

  updateVenue(venue: Venue): Observable<Venue> {
    return this.http.put<Venue>(`${this.apiUrl}/${venue.id}`, venue).pipe(
      catchError(this.handleError)
    );
  }

  deleteVenue(venueId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${venueId}`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
