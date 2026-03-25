import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { Venue } from '../models/venue';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class VenueService {
  private apiUrl = `${environment.baseApiUrl}/api/venues`;
  private venuesSource = new BehaviorSubject<Venue[]>([]);
  venues$ = this.venuesSource.asObservable();

  constructor(private http: HttpClient) {}

  getVenue(venueId: string): Observable<Venue> {
    return this.http.get<Venue>(`${this.apiUrl}/${venueId}`).pipe(
      tap(venue => {
        const currentVenues = this.venuesSource.getValue();
        if (!currentVenues.find(v => v.id === venue.id))
        {
          currentVenues.push(venue);
          this.venuesSource.next([...currentVenues]);
        }
      }),
      catchError(this.handleError)
    );
  }

  getVenuesByOrganizerId(organizerId: string): Observable<Venue[]> {
    return this.http.get<Venue[]>(`${this.apiUrl}/organizers/${organizerId}`).pipe(
      tap(venues => this.venuesSource.next(venues)),
      catchError(this.handleError)
    );
  }

  postVenue(venue: Venue): Observable<Venue> {
    return this.http.post<Venue>(this.apiUrl, venue).pipe(
      tap(newVenue => {
        const updatedVenues = [...this.venuesSource.getValue(), newVenue];
        this.venuesSource.next(updatedVenues);
      }),
      catchError(this.handleError)
    );
  }

  updateVenue(venue: Venue): Observable<Venue> {
    return this.http.put<Venue>(`${this.apiUrl}/${venue.id}`, venue).pipe(
      tap(updatedVenue => {
        const venues = this.venuesSource.getValue();
        const index = venues.findIndex(v => v.id === updatedVenue.id);
        if (index !== -1)
        {
          venues[index] = updatedVenue;
          this.venuesSource.next([...venues]);
        }
      }),
      catchError(this.handleError)
    );
  }

  deleteVenueById(venueId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${venueId}`).pipe(
      tap(() => {
        const venues = this.venuesSource.getValue().filter(v => v.id !== venueId);
        this.venuesSource.next(venues);
      }),
      catchError(this.handleError)
    );
  }

  removeAuditoriumFromVenue(venueId: string, auditoriumId: string): void {
    const currentVenues = this.venuesSource.value;

    const updatedVenues = currentVenues.map(venue => {
      if (venue.id === venueId)
      {
        return { ...venue, auditoriums: venue.auditoriums.filter(a => a.id !== auditoriumId) };
      }
      return venue;
    });

    this.venuesSource.next(updatedVenues);
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
