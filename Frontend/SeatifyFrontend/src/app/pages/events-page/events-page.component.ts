import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { EventCard } from '../../models/event-card';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-events-page',
  standalone: false,
  templateUrl: './events-page.component.html',
  styleUrl: './events-page.component.sass'
})
export class EventsPageComponent implements OnInit, OnDestroy {
  events: EventCard[] = [];
  isLoading: boolean = true;
  errorMessage: string = '';
  private readonly unsubscribe$ = new Subject<void>();

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvents();
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  loadEvents(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.eventService.getEventCards().pipe(takeUntil(this.unsubscribe$)).subscribe({
      next: events => {
        this.events = events;
        this.isLoading = false;
      },
      error: error => {
        this.errorMessage = error.message;
        this.isLoading = false;
      }
    });
  }

  addEvent(): void {
    console.log('Add new event');
  }

  editSeatMap(event: EventCard): void {
    console.log('Edit seat map', event);
  }

  editDetails(event: EventCard): void {
    console.log('Edit details', event);
  }

  viewPublicPage(event: EventCard): void {
    console.log('View public page', event);
  }
}
