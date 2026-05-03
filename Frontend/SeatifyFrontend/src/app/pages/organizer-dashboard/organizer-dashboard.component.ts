import { Component, OnDestroy, OnInit } from '@angular/core';
import { filter, Subject, takeUntil } from 'rxjs';
import { EventService } from '../../services/event.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: false,
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.sass']
})
export class OrganizerDashboardComponent implements OnInit, OnDestroy {
  activeEventsCount = 0;

  private readonly destroy$ = new Subject<void>();

  constructor(private readonly eventService: EventService, private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$
      .pipe(filter((u): u is NonNullable<typeof u> => !!u))
      .subscribe(user => this.loadActiveEventsCount(user.organizerId));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadActiveEventsCount(organizerId: any): void {
    this.eventService
      .getActiveEventsCount(organizerId)
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
