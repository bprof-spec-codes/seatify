import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { EventCard, EventCardOccurrence } from '../../models/event-card';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-events-page',
  standalone: false,
  templateUrl: './events-page.component.html',
  styleUrls: ['./events-page.component.sass'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EventsPageComponent implements OnInit, OnDestroy {
  events: EventCard[] = [];
  isLoading = false;
  errorMessage = '';

  private readonly destroy$ = new Subject<void>();

  constructor(private readonly eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.eventService
      .getEventCards()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (events) => {
          this.events = events;
          this.isLoading = false;
        },
        error: (error: Error) => {
          this.errorMessage = error.message || 'Failed to load events.';
          this.isLoading = false;
        }
      });
  }

  trackByEventId(index: number, event: EventCard): string {
    return event.id;
  }

  trackByOccurrenceId(index: number, occurrence: EventCardOccurrence): string {
    return occurrence.id;
  }

  addEvent(): void {
    console.log('TODO: open create event page or modal');
  }

  editSeatMap(event: EventCard): void {
    console.log('TODO: navigate to seat map editor', event.id);
  }

  editDetails(event: EventCard): void {
    console.log('TODO: navigate to event details editor', event.id);
  }

  viewPublicPage(event: EventCard): void {
    console.log('TODO: navigate to public event page', event.slug);
  }
}