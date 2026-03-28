import { Injectable } from '@angular/core';
import { Sector } from '../models/sector';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { environment } from '../../environments/environment.development';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SectorService {
  private apiUrl = `${environment.baseApiUrl}/api`
  private SectorSource = new BehaviorSubject<Sector[]>([])
  Sector$ = this.SectorSource.asObservable()

  constructor(private http: HttpClient) { }

  getSectorsByAuditoriumId(auditoriumId: string): Observable<Sector[]> {
    return this.http.get<Sector[]>(`${this.apiUrl}/auditoriums/${auditoriumId}/sectors`).pipe(
      map(sectors => sectors.map(sector => this.mapSectorDates(sector))),
      tap(sectors => this.SectorSource.next(sectors)),
      catchError(this.handleError)
    )
  }

  setSectors(sectors: Sector[]): void {
    this.SectorSource.next(sectors)
  }

  clearSectors(): void {
    this.SectorSource.next([])
  }

  private mapSectorDates(sector: Sector): Sector {
    return {
      ...sector,
      createdAtUtc: new Date(sector.createdAtUtc),
      updatedAtUtc: new Date(sector.updatedAtUtc)
    }
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message)
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }

}
