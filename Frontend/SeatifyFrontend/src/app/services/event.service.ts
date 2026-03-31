import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, forkJoin, map, of, switchMap, throwError } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { SeatifyEvent } from '../models/event';
import { EventCard } from '../models/event-card';
import { EventOccurrence } from '../models/event-occurrence';
import EventRequest from '../models/event.request';
import EventResponse from '../models/event.response';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly apiUrl = `${environment.baseApiUrl}/api`;
  private readonly eventsApiUrl = `${this.apiUrl}/event`;
  private readonly eventOccurrencesApiUrl = `${this.apiUrl}/event-occurrences`;

  constructor(private readonly http: HttpClient) {}

  uploadImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(`${this.apiUrl}/upload`, formData);
  }

  createEvent(eventrequest: EventRequest): Observable<EventResponse>{
    return this.http.post<EventResponse>(this.apiUrl + '/event', eventrequest);
  }
  
  updateEvent(eventrequest: EventRequest, id: number): Observable<EventResponse>{
    return this.http.put<EventResponse>(`${this.apiUrl}/event/${id}`, eventrequest);
  }

  getEventById(id: string): Observable<SeatifyEvent> {
    return this.http.get<SeatifyEvent>(`${this.apiUrl}/event/${id}`);
  }

  getOccurrenceById(id: string): Observable<EventOccurrence> {
    return this.http.get<EventOccurrence>(`${this.eventOccurrencesApiUrl}/${id}`);
  }

  createOccurrence(occurrence: Partial<EventOccurrence>): Observable<any> {
    return this.http.post(this.eventOccurrencesApiUrl, occurrence);
  }

  updateOccurrence(id: string, occurrence: Partial<EventOccurrence>): Observable<any> {
    return this.http.put(`${this.eventOccurrencesApiUrl}/${id}`, occurrence);
  }

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
      map(occurrences => {
        const sortedOccurrences = [...occurrences].sort(
          (a, b) =>
            new Date(a.startsAtUtc).getTime() - new Date(b.startsAtUtc).getTime()
        );

        return this.toEventCard(event, sortedOccurrences);
      }),
      catchError(() => of(this.toEventCard(event, [])))
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

  private toEventCard(event: SeatifyEvent, occurrences: EventOccurrence[]): EventCard {
    const firstOccurrence = occurrences[0];

    return {
      id: event.id,
      slug: event.slug,
      title: event.name,
      description: event.description,
      status: event.status,
      venueName: firstOccurrence?.venue?.name ?? 'No venue assigned',
      auditoriumName: firstOccurrence?.auditorium?.name ?? '',
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

  getEvents(): Observable<SeatifyEvent[]> {
    return this.http.get<SeatifyEvent[]>(this.eventsApiUrl).pipe(
      map(events => events ?? []),
      catchError(error => this.handleFatalError(error))
    );
  }

  getActiveEventsCount(): Observable<number> {
    return this.getEvents().pipe(
      map(events => events.filter(event => event.status === 'Published').length)
    );
  }

  getAllEventsCount(): Observable<number> {
    return this.getEvents().pipe(
      map(events => events.length)
    );
  }

  private handleFatalError(error: HttpErrorResponse): Observable<never> {
    console.error('Failed to load events page.', error);
    return throwError(() => new Error('Failed to load events from the backend.'));
  }
}
