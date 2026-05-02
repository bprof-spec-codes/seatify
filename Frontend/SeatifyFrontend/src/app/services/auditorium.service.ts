import { Injectable } from '@angular/core';
import { Auditorium } from '../models/auditorium';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuditoriumService {
  private apiUrl = `${environment.baseApiUrl}/api/auditoriums`;
  private venuesApiUrl = `${environment.baseApiUrl}/api/venues`;
  private auditoriumsSource$ = new BehaviorSubject<Auditorium[]>([]);
  auditoriums$ = this.auditoriumsSource$.asObservable();

  private editModeSource = new BehaviorSubject<boolean>(false);
  editMode$ = this.editModeSource.asObservable();

  private editAuditoriumSource = new BehaviorSubject<Auditorium>(new Auditorium());
  editAuditorium$ = this.editAuditoriumSource.asObservable();

  constructor(private http: HttpClient) {}

  getAuditoriumById(auditoriumId: string): Observable<Auditorium> {
    return this.http.get<Auditorium>(`${this.apiUrl}/${auditoriumId}`).pipe(
      catchError(this.handleError)
    );
  }

  getAuditoriumsByVenueId(venueId: string): Observable<Auditorium[]> {
    return this.http.get<Auditorium[]>(`${this.venuesApiUrl}/${venueId}/auditoriums`).pipe(
      catchError(this.handleError)
    );
  }

  createAuditorium(venueId: string, auditorium: Auditorium): Observable<Auditorium> {
    return this.http.post<Auditorium>(`${this.venuesApiUrl}/${venueId}/auditoriums`, auditorium).pipe(
      tap(newAuditorium => {
        const updatedAuditoriums = [...this.auditoriumsSource$.getValue(), newAuditorium];
        this.auditoriumsSource$.next(updatedAuditoriums);
      }),
      catchError(this.handleError)
    );
  }

  updateAuditorium(auditorium: Auditorium): Observable<Auditorium> {
    return this.http.put<Auditorium>(`${this.apiUrl}/${auditorium.id}`, auditorium).pipe(
      tap(updatedAuditorium => {
        const auditoriums = this.auditoriumsSource$.getValue();
        const index = auditoriums.findIndex(a => a.id === updatedAuditorium.id);
        if (index !== -1) {
          auditoriums[index] = { ...auditoriums[index], ...updatedAuditorium };
          this.auditoriumsSource$.next([...auditoriums]);
        }
      }),
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

  getEditMode(): Observable<boolean> {
    return this.editMode$;
  }

  setEditMode(editMode: boolean): void {
    this.editModeSource.next(editMode);
  }

  getEditAuditorium(): Observable<Auditorium> {
    return this.editAuditorium$;
  }

  setEditAuditorium(editAuditorium: Auditorium): void {
    this.editAuditoriumSource.next(editAuditorium);
  }

  checkAuditoriumHasBookings(auditoriumId: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/${auditoriumId}/has-bookings`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message);
    if (error.error) {
        console.error('Detailed error from backend: ', error.error);
    }
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
