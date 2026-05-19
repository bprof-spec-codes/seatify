import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  BulkSeatOverrideDto,
  BulkSeatOverrideResponseDto,
  EffectiveSeatMap
} from '../models/seat-override';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class SeatOverrideService {
  private readonly apiUrl = `${environment.baseApiUrl}/api`;

  private readonly seatOverridePath = '/api';

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) { }

  // ─── Event szintű ───────────────────────────────────────────────────────────

  private api(path: string): string {
    return `${this.configService.cfg.baseApiUrl}${path}`;
  }

  getEffectiveSeatMapForEvent(eventId: string, matrixId: string): Observable<EffectiveSeatMap> {
    return this.http
      .get<EffectiveSeatMap>(`${this.api(this.seatOverridePath)}/events/${eventId}/seat-map/${matrixId}`)
      .pipe(catchError(this.handleError));
  }

  bulkUpsertEventOverride(eventId: string, dto: BulkSeatOverrideDto): Observable<BulkSeatOverrideResponseDto> {
    return this.http
      .patch<BulkSeatOverrideResponseDto>(`${this.api(this.seatOverridePath)}/events/${eventId}/seats/bulk`, dto)
      .pipe(catchError(this.handleError));
  }

  // ─── Occurrence szintű ──────────────────────────────────────────────────────

  getEffectiveSeatMapForOccurrence(occurrenceId: string, matrixId: string): Observable<EffectiveSeatMap> {
    return this.http
      .get<EffectiveSeatMap>(`${this.api(this.seatOverridePath)}/event-occurrences/${occurrenceId}/seat-map/${matrixId}`)
      .pipe(catchError(this.handleError));
  }

  bulkUpsertOccurrenceOverride(occurrenceId: string, dto: BulkSeatOverrideDto): Observable<BulkSeatOverrideResponseDto> {
    return this.http
      .patch<BulkSeatOverrideResponseDto>(`${this.api(this.seatOverridePath)}/event-occurrences/${occurrenceId}/seats/bulk`, dto)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('SeatOverrideService error:', error);
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }
}
