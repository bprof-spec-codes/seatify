import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';

import { Venue } from '../models/venue';
import { Auditorium } from '../models/auditorium';
import { environment } from '../../environments/environment';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class VenueService {
  private readonly venuesPath = '/api/venues';

  private venuesSource = new BehaviorSubject<Venue[]>([]);
  venues$ = this.venuesSource.asObservable();

  private editModeSource = new BehaviorSubject<boolean>(false);
  editMode$ = this.editModeSource.asObservable();

  private editVenueSource = new BehaviorSubject<Venue>(new Venue());
  editVenue$ = this.editVenueSource.asObservable();

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) { }

  private get baseApiUrl(): string {
    const configuredBaseUrl = this.configService?.cfg?.baseApiUrl;
    const fallbackBaseUrl = environment.baseApiUrl;

    return (configuredBaseUrl || fallbackBaseUrl || '').replace(/\/$/, '');
  }

  private api(path: string): string {
    return `${this.baseApiUrl}${path}`;
  }

  getVenueById(venueId: string): Observable<Venue> {
    return this.http.get<Venue>(`${this.api(this.venuesPath)}/${venueId}`).pipe(
      tap(venue => {
        const currentVenues = this.venuesSource.getValue();

        if (!currentVenues.find(v => v.id === venue.id)) {
          this.venuesSource.next([...currentVenues, venue]);
        }
      }),
      catchError(this.handleError)
    );
  }

  loadVenuesByOrganizerId(organizerId: string): void {
    this.http.get<Venue[]>(`${this.api(this.venuesPath)}/organizers/${organizerId}`).pipe(
      catchError(this.handleError)
    ).subscribe(venues => {
      this.venuesSource.next(venues);
    });
  }

  getVenuesByOrganizerId(organizerId: string): Observable<Venue[]> {
    return this.http.get<Venue[]>(`${this.api(this.venuesPath)}/organizers/${organizerId}`).pipe(
      tap(venues => this.venuesSource.next(venues)),
      catchError(this.handleError)
    );
  }

  createVenue(venue: Venue): Observable<Venue> {
    return this.http.post<Venue>(this.api(this.venuesPath), venue).pipe(
      tap(newVenue => {
        const updatedVenues = [...this.venuesSource.getValue(), newVenue];
        this.venuesSource.next(updatedVenues);
      }),
      catchError(this.handleError)
    );
  }

  updateVenue(venue: Venue): Observable<Venue> {
    return this.http.put<Venue>(`${this.api(this.venuesPath)}/${venue.id}`, venue).pipe(
      tap(updatedVenue => {
        const venues = this.venuesSource.getValue();

        const updatedVenues = venues.map(existingVenue =>
          existingVenue.id === updatedVenue.id
            ? { ...existingVenue, ...updatedVenue }
            : existingVenue
        );

        this.venuesSource.next(updatedVenues);
      }),
      catchError(this.handleError)
    );
  }

  deleteVenueById(venueId: string): Observable<void> {
    return this.http.delete<void>(`${this.api(this.venuesPath)}/${venueId}`).pipe(
      tap(() => {
        const updatedVenues = this.venuesSource.getValue().filter(v => v.id !== venueId);
        this.venuesSource.next(updatedVenues);
      }),
      catchError(this.handleError)
    );
  }

  removeAuditoriumFromVenue(venueId: string, auditoriumId: string): void {
    const currentVenues = this.venuesSource.getValue();

    const updatedVenues = currentVenues.map(venue => {
      if (venue.id !== venueId) {
        return venue;
      }

      return {
        ...venue,
        auditoriums: venue.auditoriums.filter(a => a.id !== auditoriumId)
      };
    });

    this.venuesSource.next(updatedVenues);
  }

  addAuditoriumToVenue(venueId: string, auditorium: Auditorium): void {
    const currentVenues = this.venuesSource.getValue();

    const updatedVenues = currentVenues.map(venue => {
      if (venue.id !== venueId) {
        return venue;
      }

      return {
        ...venue,
        auditoriums: [...venue.auditoriums, auditorium]
      };
    });

    this.venuesSource.next(updatedVenues);
  }

  updateAuditoriumInVenue(venueId: string, updatedAuditorium: Auditorium): void {
    const currentVenues = this.venuesSource.getValue();

    const updatedVenues = currentVenues.map(venue => {
      if (venue.id !== venueId) {
        return venue;
      }

      return {
        ...venue,
        auditoriums: venue.auditoriums.map(a =>
          a.id === updatedAuditorium.id ? updatedAuditorium : a
        )
      };
    });

    this.venuesSource.next(updatedVenues);
  }

  getEditMode(): Observable<boolean> {
    return this.editMode$;
  }

  setEditMode(editMode: boolean): void {
    this.editModeSource.next(editMode);
  }

  getEditVenue(): Observable<Venue> {
    return this.editVenue$;
  }

  setEditVenue(editVenue: Venue): void {
    this.editVenueSource.next(editVenue);
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred:', error.message);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}