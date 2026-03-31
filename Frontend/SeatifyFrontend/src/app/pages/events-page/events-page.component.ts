import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { EventCard, EventCardOccurrence } from '../../models/event-card';
import { EventService } from '../../services/event.service';
import { CreateEventForm } from '../../models/event';

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

  isCreateEventModalOpen = false;
  isSubmittingCreateEvent = false;

  createEventForm: CreateEventForm = {
    name: '',
    slug: '',
    description: ''
  };

  private readonly destroy$ = new Subject<void>();

  constructor(private readonly eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  @HostListener('document:keydown.escape')
  onEscapePressed(): void {
    if (this.isCreateEventModalOpen && !this.isSubmittingCreateEvent) {
      this.closeCreateEventModal();
    }
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
    this.resetCreateEventForm();
    this.isCreateEventModalOpen = true;
  }

  closeCreateEventModal(): void {
    this.isCreateEventModalOpen = false;
  }

  onModalBackdropClick(event: MouseEvent): void {
    if (event.target === event.currentTarget && !this.isSubmittingCreateEvent) {
      this.closeCreateEventModal();
    }
  }

  submitCreateEvent(): void {
    if (!this.createEventForm.name.trim() || !this.createEventForm.slug.trim()) {
      return;
    }

    this.isSubmittingCreateEvent = true;

    // TODO: ha lesz backend POST /api/event endpoint,
    // ide kell bekötni a service hívást.
    console.log('Create event payload:', {
      name: this.createEventForm.name.trim(),
      slug: this.createEventForm.slug.trim(),
      description: this.createEventForm.description.trim(),
    });

    this.isSubmittingCreateEvent = false;
    this.closeCreateEventModal();
  }

  private resetCreateEventForm(): void {
    this.createEventForm = {
      name: '',
      slug: '',
      description: '',
    };
  }

  editSeatMap(event: EventCard, occurrence: EventCardOccurrence): void {
    console.log('TODO: navigate to seat map editor', {
      eventId: event.id,
      occurrenceId: occurrence.id
    });
  }

  editDetails(event: EventCard, occurrence: EventCardOccurrence): void {
    console.log('TODO: navigate to occurrence details editor', {
      eventId: event.id,
      occurrenceId: occurrence.id
    });
  }

  viewPublicPage(event: EventCard, occurrence: EventCardOccurrence): void {
    console.log('TODO: navigate to public occurrence page', {
      slug: event.slug,
      occurrenceId: occurrence.id
    });
  }
}