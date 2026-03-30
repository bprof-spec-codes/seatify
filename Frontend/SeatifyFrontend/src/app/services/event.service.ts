import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {Observable,catchError,forkJoin,map,of,shareReplay,switchMap,throwError} from 'rxjs';
import { environment } from '../../environments/environment.development';
import { SeatifyEvent } from '../models/event';
import { EventCard } from '../models/event-card';
import { EventOccurrence } from '../models/event-occurrence';

interface ReservationSeatView {
  seatId: string;
  finalPrice: number;
}

interface ReservationView {
  id: string;
  customerName: string;
  customerEmail: string;
  status: string;
  createdAtUtc: string;
  reservedSeats: ReservationSeatView[];
}

interface LayoutMatrixView {
  id: string;
  auditoriumId: string;
  name: string;
  rows: number;
  columns: number;
  createdAtUtc: string;
  updatedAtUtc: string;
}

interface SeatView {
  id: string;
  matrixId: string;
  row: number;
  column: number;
  seatLabel?: string;
  sectorId?: string;
  priceOverride?: number;
  seatType: string;
  isBookable: boolean;
  createdAtUtc: string;
  updatedAtUtc: string;
}

interface OccurrenceMetrics {
  soldTickets: number;
  totalTickets: number;
}

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly apiUrl = `${environment.baseApiUrl}/api`;
  private readonly eventsApiUrl = `${this.apiUrl}/event`;
  private readonly eventOccurrencesApiUrl = `${this.apiUrl}/event-occurrences`;
  private readonly auditoriumCapacityCache = new Map<string, Observable<number>>();

  constructor(private readonly http: HttpClient) {}

  getEventCards(): Observable<EventCard[]> {
    return this.http.get<SeatifyEvent[]>(this.eventsApiUrl).pipe(
      switchMap(events => {
        if (!events?.length) {
          return of([]);
        }

        return forkJoin(events.map(event => this.buildEventCard(event)));
      }),
      map(cards => this.sortCardsByNextOccurrence(cards)),
      catchError(error => this.handleFatalError(error))
    );
  }

  private buildEventCard(event: SeatifyEvent): Observable<EventCard> {
    return this.getOccurrencesByEventId(event.id).pipe(
      switchMap(occurrences => {
        const sortedOccurrences = [...occurrences].sort(
          (a, b) =>
            new Date(a.startsAtUtc).getTime() - new Date(b.startsAtUtc).getTime()
        );

        if (!sortedOccurrences.length) {
          return of(this.toEventCard(event, [], 0, 0));
        }

        return forkJoin(
          sortedOccurrences.map(occurrence => this.getOccurrenceMetrics(occurrence))
        ).pipe(
          map(metrics => {
            const soldTickets = metrics.reduce((sum, item) => sum + item.soldTickets, 0);
            const totalTickets = metrics.reduce((sum, item) => sum + item.totalTickets, 0);

            return this.toEventCard(event, sortedOccurrences, soldTickets, totalTickets);
          }),
          catchError(() => of(this.toEventCard(event, sortedOccurrences, 0, 0)))
        );
      }),
      catchError(() => of(this.toEventCard(event, [], 0, 0)))
    );
  }

  private getOccurrencesByEventId(eventId: string): Observable<EventOccurrence[]> {
    return this.http
      .get<EventOccurrence[]>(`${this.eventOccurrencesApiUrl}/by-event/${eventId}`)
      .pipe(
        map(occurrences => occurrences ?? []),
        catchError(() => of([]))
      );
  }

  private getOccurrenceMetrics(occurrence: EventOccurrence): Observable<OccurrenceMetrics> {
    return forkJoin({
      reservations: this.getReservationsByOccurrenceId(occurrence.id),
      totalTickets: this.getAuditoriumCapacity(occurrence.auditoriumId)
    }).pipe(
      map(({ reservations, totalTickets }) => ({
        soldTickets: reservations.reduce(
          (sum, reservation) => sum + (reservation.reservedSeats?.length ?? 0),
          0
        ),
        totalTickets
      })),
      catchError(() => of({ soldTickets: 0, totalTickets: 0 }))
    );
  }

  private getReservationsByOccurrenceId(
    occurrenceId: string
  ): Observable<ReservationView[]> {
    return this.http
      .get<ReservationView[]>(`${this.eventOccurrencesApiUrl}/${occurrenceId}/reservations`)
      .pipe(
        map(reservations => reservations ?? []),
        catchError(() => of([]))
      );
  }

  private getAuditoriumCapacity(auditoriumId: string): Observable<number> {
    if (!auditoriumId) {
      return of(0);
    }

    const cachedCapacity = this.auditoriumCapacityCache.get(auditoriumId);
    if (cachedCapacity) {
      return cachedCapacity;
    }

    const request$ = this.http
      .get<LayoutMatrixView[]>(`${this.apiUrl}/auditoriums/${auditoriumId}/layout-matrices`)
      .pipe(
        switchMap(layoutMatrices => {
          if (!layoutMatrices?.length) {
            return of(0);
          }

          return forkJoin(
            layoutMatrices.map(layoutMatrix =>
              this.http
                .get<SeatView[]>(`${this.apiUrl}/layout-matrices/${layoutMatrix.id}/seats`)
                .pipe(catchError(() => of([])))
            )
          ).pipe(
            map(seatGroups =>
              seatGroups
                .flat()
                .filter(seat => seat.isBookable)
                .length
            )
          );
        }),
        catchError(() => of(0)),
        shareReplay(1)
      );

    this.auditoriumCapacityCache.set(auditoriumId, request$);
    return request$;
  }

  private toEventCard(
    event: SeatifyEvent,
    occurrences: EventOccurrence[],
    soldTickets: number,
    totalTickets: number
  ): EventCard {
    const firstOccurrence = occurrences[0];

    return {
      id: event.id,
      slug: event.slug,
      title: event.name,
      description: event.description,
      status: event.status,
      venueName: firstOccurrence?.venue?.name ?? 'No venue assigned',
      auditoriumName: firstOccurrence?.auditorium?.name ?? '',
      soldTickets,
      totalTickets,
      occurrences: occurrences.map(occurrence => ({
        id: occurrence.id,
        startsAtUtc: occurrence.startsAtUtc,
        endsAtUtc: occurrence.endsAtUtc,
        status: occurrence.status
      }))
    };
  }

  private sortCardsByNextOccurrence(cards: EventCard[]): EventCard[] {
    return [...cards].sort((a, b) => {
      const aDate = a.occurrences.length
        ? new Date(a.occurrences[0].startsAtUtc).getTime()
        : Number.MAX_SAFE_INTEGER;

      const bDate = b.occurrences.length
        ? new Date(b.occurrences[0].startsAtUtc).getTime()
        : Number.MAX_SAFE_INTEGER;

      return aDate - bDate;
    });
  }

  private handleFatalError(error: HttpErrorResponse): Observable<never> {
    console.error('Failed to load events page.', error);

    return throwError(
      () => new Error('Failed to load events from the backend.')
    );
  }
}