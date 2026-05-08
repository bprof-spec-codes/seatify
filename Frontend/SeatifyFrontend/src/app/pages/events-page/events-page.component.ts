import { Component, OnDestroy, OnInit } from '@angular/core';
import { filter, Subject, takeUntil } from 'rxjs';
import { Router } from '@angular/router';
import { EventCard, EventCardOccurrence } from '../../models/event-card';
import { EventService } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-events-page',
  standalone: false,
  templateUrl: './events-page.component.html',
  styleUrls: ['./events-page.component.sass']
})
export class EventsPageComponent implements OnInit, OnDestroy {
  events: EventCard[] = [];
  isLoading = false;
  errorMessage = '';

  selectedOccurrenceIds: Record<string, string> = {};

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly eventService: EventService,
    private readonly router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(filter((u): u is NonNullable<typeof u> => !!u))
      .subscribe(user => this.loadEvents(user.organizerId));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEvents(organizerId: any): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.eventService
      .getEventCards(organizerId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (events) => {
          this.events = events;
          this.initializeSelections(events);
          this.isLoading = false;
        },
        error: (error: Error) => {
          this.errorMessage = error.message || 'Failed to load events.';
          this.isLoading = false;
        }
      });
  }

  private initializeSelections(events: EventCard[]): void {
    const nextSelections: Record<string, string> = {};

    for (const event of events) {
      const currentSelectedId = this.selectedOccurrenceIds[event.id];
      const existsInList = event.occurrences.some(o => o.id === currentSelectedId);

      if (existsInList && currentSelectedId) {
        nextSelections[event.id] = currentSelectedId;
      } else if (event.occurrences.length > 0) {
        nextSelections[event.id] = event.occurrences[0].id;
      }
    }

    this.selectedOccurrenceIds = nextSelections;
  }

  selectOccurrence(event: EventCard, occurrence: EventCardOccurrence): void {
    this.selectedOccurrenceIds[event.id] = occurrence.id;
  }

  isOccurrenceSelected(event: EventCard, occurrence: EventCardOccurrence): boolean {
    return this.selectedOccurrenceIds[event.id] === occurrence.id;
  }

  getSelectedOccurrence(event: EventCard): EventCardOccurrence | null {
    const selectedOccurrenceId = this.selectedOccurrenceIds[event.id];

    if (!selectedOccurrenceId) {
      return null;
    }

    return event.occurrences.find(o => o.id === selectedOccurrenceId) ?? null;
  }

  trackByEventId(index: number, event: EventCard): string {
    return event.id;
  }

  trackByOccurrenceId(index: number, occurrence: EventCardOccurrence): string {
    return occurrence.id;
  }

  addEvent(): void {
    this.router.navigate(['/dashboard/events/new']);
  }

  editEvent(event: EventCard): void {
    this.router.navigate(['/dashboard/events', event.id, 'edit']);
  }


  editSeatMap(event: EventCard, occurrence: EventCardOccurrence): void {
    this.router.navigate(
      ['/layout-matrix', occurrence.auditoriumId, 'editor'],
      { queryParams: { occurrenceId: occurrence.id, eventId: event.id } }
    );
  }

  editDetails(event: EventCard, occurrence: EventCardOccurrence): void {
    this.router.navigate(['/dashboard/events', event.id, 'occurrences', occurrence.id, 'edit']);
  }

  addDate(event: EventCard): void {
    this.router.navigate(['/dashboard/events', event.id, 'occurrences', 'new']);
  }

  viewPublicPage(event: EventCard, occurrence: EventCardOccurrence, eventObj: MouseEvent): void {
    eventObj.stopPropagation();
    const url = this.router.serializeUrl(
      this.router.createUrlTree(['/book', event.slug, occurrence.id])
    );
    window.open(url, '_blank');
  }

  copyPublicLink(event: EventCard, occurrence: EventCardOccurrence, eventObj: MouseEvent): void {
    eventObj.stopPropagation();
    const basePath = window.location.origin;
    const path = this.router.serializeUrl(
      this.router.createUrlTree(['/book', event.slug, occurrence.id])
    );
    const fullUrl = `${basePath}${path}`;
    
    navigator.clipboard.writeText(fullUrl).then(() => {
      // Could show a toast notification here
      alert('Link copied to clipboard!');
    });
  }
}
