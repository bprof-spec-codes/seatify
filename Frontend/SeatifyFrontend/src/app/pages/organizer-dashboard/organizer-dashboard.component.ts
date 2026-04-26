import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, forkJoin, of, Subject, takeUntil } from 'rxjs';
import { EventService } from '../../services/event.service';
import { VenueService } from '../../services/venue.service';
import { EventCard, EventCardOccurrence } from '../../models/event-card';
import { SeatifyEvent } from '../../models/event';
import { Venue } from '../../models/venue';

type CardShape = 'soft' | 'solid' | 'glass';
type DashboardDensity = 'comfortable' | 'compact';

interface AppearancePreset {
  id: string;
  name: string;
  description: string;
  icon: string;
  primaryColor: string;
  accentColor: string;
  backgroundColor: string;
  surfaceColor: string;
  textColor: string;
  cardShape: CardShape;
  density: DashboardDensity;
}

interface DashboardStat {
  label: string;
  value: string | number;
  helper: string;
  icon: string;
  tone: 'primary' | 'success' | 'warning' | 'info';
}

interface UpcomingEventViewModel {
  eventId: string;
  title: string;
  venueName: string;
  auditoriumName: string;
  occurrence: EventCardOccurrence;
}

@Component({
  selector: 'app-organizer-dashboard',
  standalone: false,
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.sass']
})
export class OrganizerDashboardComponent implements OnInit, OnDestroy {
  readonly organizerId = 'org-id-01';
  readonly storageKey = 'seatify-organizer-dashboard-appearance';

  isLoading = false;
  errorMessage = '';

  activeEventsCount = 0;
  allEventsCount = 0;
  venuesCount = 0;
  auditoriumCount = 0;
  draftEventsCount = 0;

  upcomingEvents: UpcomingEventViewModel[] = [];
  stats: DashboardStat[] = [];

  selectedPresetId = 'ocean';
  appearance: AppearancePreset = this.getDefaultAppearance();

  readonly appearancePresets: AppearancePreset[] = [
    {
      id: 'ocean',
      name: 'Ocean Blue',
      description: 'Clean, bright dashboard for everyday organizer work.',
      icon: 'bi-droplet-half',
      primaryColor: '#3b82f6',
      accentColor: '#0ea5e9',
      backgroundColor: '#f1f5f9',
      surfaceColor: '#ffffff',
      textColor: '#0f172a',
      cardShape: 'soft',
      density: 'comfortable'
    },
    {
      id: 'midnight',
      name: 'Midnight Pro',
      description: 'Dark, premium look for event control rooms.',
      icon: 'bi-moon-stars',
      primaryColor: '#8b5cf6',
      accentColor: '#22d3ee',
      backgroundColor: '#0f172a',
      surfaceColor: '#1e293b',
      textColor: '#f8fafc',
      cardShape: 'glass',
      density: 'comfortable'
    },
    {
      id: 'forest',
      name: 'Forest',
      description: 'Calm green interface for venues and cultural events.',
      icon: 'bi-tree',
      primaryColor: '#10b981',
      accentColor: '#84cc16',
      backgroundColor: '#ecfdf5',
      surfaceColor: '#ffffff',
      textColor: '#064e3b',
      cardShape: 'soft',
      density: 'comfortable'
    },
    {
      id: 'sunset',
      name: 'Sunset',
      description: 'Warmer colors for festivals, concerts and nightlife.',
      icon: 'bi-sunset',
      primaryColor: '#f97316',
      accentColor: '#f43f5e',
      backgroundColor: '#fff7ed',
      surfaceColor: '#ffffff',
      textColor: '#431407',
      cardShape: 'solid',
      density: 'compact'
    }
  ];

  readonly cardShapeOptions: { value: CardShape; label: string }[] = [
    { value: 'soft', label: 'Soft cards' },
    { value: 'solid', label: 'Strong cards' },
    { value: 'glass', label: 'Glass cards' }
  ];

  readonly densityOptions: { value: DashboardDensity; label: string }[] = [
    { value: 'comfortable', label: 'Comfortable' },
    { value: 'compact', label: 'Compact' }
  ];

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly eventService: EventService,
    private readonly venueService: VenueService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.loadSavedAppearance();
    this.loadDashboardData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get appearanceStyles(): Record<string, string> {
    return {
      '--dashboard-primary': this.appearance.primaryColor,
      '--dashboard-accent': this.appearance.accentColor,
      '--dashboard-bg': this.appearance.backgroundColor,
      '--dashboard-surface': this.appearance.surfaceColor,
      '--dashboard-text': this.appearance.textColor
    };
  }

  selectPreset(preset: AppearancePreset): void {
    this.selectedPresetId = preset.id;
    this.appearance = { ...preset };
    this.saveAppearance();
  }

  updateAppearance(): void {
    this.selectedPresetId = 'custom';
    this.appearance = {
      ...this.appearance,
      id: 'custom',
      name: 'Custom Theme'
    };

    this.saveAppearance();
  }

  resetAppearance(): void {
    this.selectPreset(this.getDefaultAppearance());
  }

  openCheckInModal(modal: { openModal: () => void }): void {
    modal.openModal();
  }

  goToEvents(): void {
    this.router.navigate(['/dashboard/events']);
  }

  goToVenues(): void {
    this.router.navigate(['/dashboard/venues']);
  }

  trackByPresetId(index: number, preset: AppearancePreset): string {
    return preset.id;
  }

  trackByStatLabel(index: number, stat: DashboardStat): string {
    return stat.label;
  }

  trackByUpcomingEvent(index: number, item: UpcomingEventViewModel): string {
    return `${item.eventId}-${item.occurrence.id}`;
  }

  private loadDashboardData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      events: this.eventService.getEvents().pipe(
        catchError(() => of([] as SeatifyEvent[]))
      ),
      eventCards: this.eventService.getEventCards().pipe(
        catchError(() => of([] as EventCard[]))
      ),
      venues: this.venueService.getVenuesByOrganizerId(this.organizerId).pipe(
        catchError(() => of([] as Venue[]))
      )
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ events, eventCards, venues }) => {
          this.activeEventsCount = events.filter(event => event.status === 'Published').length;
          this.draftEventsCount = events.filter(event => event.status === 'Draft').length;
          this.allEventsCount = events.length;
          this.venuesCount = venues.length;
          this.auditoriumCount = venues.reduce(
            (sum, venue) => sum + (venue.auditoriums?.length ?? 0),
            0
          );

          this.upcomingEvents = this.buildUpcomingEvents(eventCards);
          this.stats = this.buildStats();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Dashboard data could not be loaded.';
          this.stats = this.buildStats();
          this.isLoading = false;
        }
      });
  }

  private buildStats(): DashboardStat[] {
    return [
      {
        label: 'Active Events',
        value: this.activeEventsCount,
        helper: `${this.draftEventsCount} draft event${this.draftEventsCount === 1 ? '' : 's'}`,
        icon: 'bi-calendar-event',
        tone: 'primary'
      },
      {
        label: 'All Events',
        value: this.allEventsCount,
        helper: 'Total event catalog',
        icon: 'bi-collection',
        tone: 'info'
      },
      {
        label: 'Venues',
        value: this.venuesCount,
        helper: `${this.auditoriumCount} auditorium${this.auditoriumCount === 1 ? '' : 's'}`,
        icon: 'bi-building',
        tone: 'success'
      },
      {
        label: 'Check-In',
        value: 'Scan',
        helper: 'Open QR validation modal',
        icon: 'bi-qr-code-scan',
        tone: 'warning'
      }
    ];
  }

  private buildUpcomingEvents(eventCards: EventCard[]): UpcomingEventViewModel[] {
    const now = new Date().getTime();

    return eventCards
      .flatMap(event =>
        event.occurrences
          .filter(occurrence => new Date(occurrence.startsAtUtc).getTime() >= now)
          .map(occurrence => ({
            eventId: event.id,
            title: event.title,
            venueName: event.venueName,
            auditoriumName: event.auditoriumName,
            occurrence
          }))
      )
      .sort((a, b) =>
        new Date(a.occurrence.startsAtUtc).getTime() -
        new Date(b.occurrence.startsAtUtc).getTime()
      )
      .slice(0, 4);
  }

  private loadSavedAppearance(): void {
    const saved = localStorage.getItem(this.storageKey);

    if (!saved) {
      this.appearance = this.getDefaultAppearance();
      this.selectedPresetId = this.appearance.id;
      return;
    }

    try {
      const parsed = JSON.parse(saved) as AppearancePreset;
      this.appearance = {
        ...this.getDefaultAppearance(),
        ...parsed
      };
      this.selectedPresetId = parsed.id ?? 'custom';
    } catch {
      this.appearance = this.getDefaultAppearance();
      this.selectedPresetId = this.appearance.id;
    }
  }

  private saveAppearance(): void {
    localStorage.setItem(this.storageKey, JSON.stringify(this.appearance));
  }

  private getDefaultAppearance(): AppearancePreset {
    return {
      id: 'ocean',
      name: 'Ocean Blue',
      description: 'Clean, bright dashboard for everyday organizer work.',
      icon: 'bi-droplet-half',
      primaryColor: '#3b82f6',
      accentColor: '#0ea5e9',
      backgroundColor: '#f1f5f9',
      surfaceColor: '#ffffff',
      textColor: '#0f172a',
      cardShape: 'soft',
      density: 'comfortable'
    };
  }
}