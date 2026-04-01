import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { environment } from '../../environments/environment.development';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { CreateUpdateSectorDto, Sector, SectorViewDto } from '../models/sector';

@Injectable({
  providedIn: 'root'
})
export class SectorService {
  private apiUrl = `${environment.baseApiUrl}/api`
  private sectorSource = new BehaviorSubject<Sector[]>([])
  sector$ = this.sectorSource.asObservable()

  constructor(private http: HttpClient) { }

  getSectorsByAuditoriumId(auditoriumId: string): Observable<Sector[]> {
    return this.http.get<Sector[]>(`${this.apiUrl}/auditoriums/${auditoriumId}/sectors`).pipe(
      map(sectors => sectors.map(Sector => this.mapSectorDates(Sector))),
      tap(sectors => this.sectorSource.next(sectors)),
      catchError(this.handleError)
    )
  }

  getSectorById(id: string): Observable<Sector> {
    return this.http.get<SectorViewDto>(`${this.apiUrl}/sectors/${id}`).pipe(
      map(sector => this.mapSector(sector)),
      catchError(this.handleError)
    )
  }

  createSector(auditoriumId: string, dto: CreateUpdateSectorDto): Observable<Sector> {
    return this.http.post<SectorViewDto>(`${this.apiUrl}/auditoriums/${auditoriumId}/sectors`, dto).pipe(
      map(sector => this.mapSector(sector)),
      tap(createdSector => {
        const current = this.sectorSource.value
        this.sectorSource.next([...current, createdSector])
      }),
      catchError(this.handleError)
    )
  }

  updateSector(id: string, dto: CreateUpdateSectorDto): Observable<Sector> {
    return this.http.put<SectorViewDto>(`${this.apiUrl}/sectors/${id}`, dto).pipe(
      map(sector => this.mapSector(sector)),
      tap(updatedSector => {
        const updated = this.sectorSource.value.map(sector =>
          sector.id === updatedSector.id ? updatedSector : sector
        )
        this.sectorSource.next(updated)
      }),
      catchError(this.handleError)
    )
  }

  deleteSector(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/sectors/${id}`).pipe(
      tap(() => {
        const filtered = this.sectorSource.value.filter(sector => sector.id !== id)
        this.sectorSource.next(filtered)
      }),
      catchError(this.handleError)
    )
  }

  setSectors(sectors: Sector[]): void {
    this.sectorSource.next(sectors)
  }

  clearSectors(): void {
    this.sectorSource.next([])
  }

  private mapSectorDates(Sector: Sector): Sector {
    return {
      ...Sector,
      createdAtUtc: new Date(Sector.createdAtUtc),
      updatedAtUtc: new Date(Sector.updatedAtUtc)
    }
  }

  private mapSector(dto: SectorViewDto): Sector {
    const sector = new Sector()
    sector.id = dto.id
    sector.auditoriumId = dto.auditoriumId
    sector.name = dto.name
    sector.color = dto.color ?? '#FFFFFF'
    sector.basePrice = dto.basePrice
    sector.createdAtUtc = new Date(dto.createdAtUtc)
    sector.updatedAtUtc = new Date(dto.updatedAtUtc)
    return sector
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message)
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }

}
