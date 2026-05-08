import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: false,
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.sass']
})
export class OrganizerDashboardComponent implements OnInit, OnDestroy {
  activeEventsCount = 0;

  private readonly destroy$ = new Subject<void>();

  constructor(private readonly eventService: EventService) {}

  ngOnInit(): void {
    this.loadActiveEventsCount();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadActiveEventsCount(): void {
    this.eventService
      .getActiveEventsCount()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (count) => {
          this.activeEventsCount = count;
        },
        error: (error) => {
          console.error('Failed to load active events count.', error);
          this.activeEventsCount = 0;
        }
      });
  }
}
