import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, forkJoin, of, Subject, takeUntil } from 'rxjs';
import { EventService } from '../../services/event.service';
import { VenueService } from '../../services/venue.service';
import { AppearanceService } from '../../services/appearance.service';
import { AuthService } from '../../services/auth.service';
import { Appearance, AppearanceCreateRequest } from '../../models/appearance';
import { EventCard, EventCardOccurrence } from '../../models/event-card';
import { SeatifyEvent } from '../../models/event';
import { Venue } from '../../models/venue';

// Removed CardShape and DashboardDensity types

interface AppearancePreset {
  id: string;
  name: string;
  description: string;
  icon: string;
  fontFamily: string;
  primaryColor: string;
  accentColor: string;
  backgroundColor: string;
  surfaceColor: string;
  textColor: string;
  secondaryColor: string;
  bannerImageUrl: string;
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

  appearance: AppearancePreset = this.getDefaultAppearance();

  readonly fontFamilyOptions: { value: string; label: string }[] = [
    { value: 'Inter', label: 'Inter (Modern)' },
    { value: 'Roboto', label: 'Roboto (Clean)' },
    { value: 'Outfit', label: 'Outfit (Trendy)' },
    { value: "'Playfair Display'", label: 'Playfair (Elegant)' },
    { value: "'Fira Code'", label: 'Fira Code (Tech)' }
  ];

  savedAppearances: Appearance[] = [];
  selectedAppearance: Appearance | null = null;

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly eventService: EventService,
    private readonly venueService: VenueService,
    private readonly appearanceService: AppearanceService,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    // For development, always force a fresh login to ensure token is valid after backend restarts
    this.isLoading = true;
    this.authService.loginAsDev().subscribe({
      next: () => {
        this.loadSavedAppearances();
        this.loadDashboardData();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Authentication failed. Please log in manually.';
        console.error('Dev login failed:', err);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }




  selectSavedTheme(theme: Appearance): void {
    this.selectedAppearance = theme;
    this.appearance = {
      ...this.appearance,
      ...theme
    };
  }

  createNewTheme(): void {
    const defaultTheme = this.getDefaultAppearance();
    
    const request: AppearanceCreateRequest = {
      name: 'New Custom Theme',
      fontFamily: defaultTheme.fontFamily,
      primaryColor: defaultTheme.primaryColor,
      accentColor: defaultTheme.accentColor,
      backgroundColor: defaultTheme.backgroundColor,
      surfaceColor: defaultTheme.surfaceColor,
      textColor: defaultTheme.textColor,
      secondaryColor: defaultTheme.secondaryColor,
      logoImageUrl: '',
      bannerImageUrl: '',
      themePreset: 'Custom',
      isDefault: this.savedAppearances.length === 0
    };

    this.appearanceService.create(request).subscribe({
      next: (newTheme) => {
        this.savedAppearances.push(newTheme);
        this.selectSavedTheme(newTheme);
        this.loadSavedAppearances();
      },
      error: (err) => {
        this.errorMessage = 'Failed to create new theme.';
        console.error(err);
      }
    });
  }

  previewTheme(): void {
    if (!this.selectedAppearance) return;
    
    // Use a seeded occurrence or the first available one as the mock context
    const occId = this.upcomingEvents.length > 0 ? this.upcomingEvents[0].occurrence.id : 'occ-01';
    
    const url = this.router.serializeUrl(
      this.router.createUrlTree(['/book', 'preview', occId], { queryParams: { themeId: this.selectedAppearance.id } })
    );
    window.open(url, '_blank');
  }

  updateAppearance(): void {
    if (!this.selectedAppearance) return;

    const request: AppearanceCreateRequest = {
      name: this.appearance.name,
      fontFamily: this.appearance.fontFamily,
      primaryColor: this.appearance.primaryColor,
      accentColor: this.appearance.accentColor,
      backgroundColor: this.appearance.backgroundColor,
      surfaceColor: this.appearance.surfaceColor,
      textColor: this.appearance.textColor,
      secondaryColor: this.appearance.secondaryColor,
      logoImageUrl: this.selectedAppearance.logoImageUrl,
      bannerImageUrl: this.appearance.bannerImageUrl,
      themePreset: this.selectedAppearance.themePreset,
      isDefault: this.selectedAppearance.isDefault
    };

    this.appearanceService.update(this.selectedAppearance.id, request).subscribe({
      next: (updated) => {
        this.selectedAppearance = updated;
        const index = this.savedAppearances.findIndex(a => a.id === updated.id);
        if (index !== -1) this.savedAppearances[index] = updated;
      },
      error: (err) => {
        this.errorMessage = 'Failed to update theme.';
        console.error(err);
      }
    });
  }

  setThemeAsDefault(theme: Appearance): void {
    this.appearanceService.setDefault(theme.id).subscribe({
      next: () => this.loadSavedAppearances(),
      error: (err) => {
        this.errorMessage = 'Failed to set theme as default.';
        console.error(err);
      }
    });
  }

  deleteTheme(theme: Appearance): void {
    this.appearanceService.delete(theme.id).subscribe({
      next: () => this.loadSavedAppearances(),
      error: (err) => {
        this.errorMessage = 'Failed to delete theme.';
        console.error(err);
      }
    });
  }

  resetAppearance(): void {
    this.appearance = this.getDefaultAppearance();
    this.selectedAppearance = null;
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

  private loadSavedAppearances(): void {
    this.appearanceService.getMyAppearances().subscribe({
      next: (apps) => {
        console.log('Saved appearances loaded on Dashboard:', apps);
        this.savedAppearances = apps;
        const defaultTheme = apps.find(t => t.isDefault);
        if (defaultTheme) {
          this.selectedAppearance = defaultTheme;
          this.appearance = {
            ...this.getDefaultAppearance(),
            ...defaultTheme
          };
        }
      }
    });
  }

  private getDefaultAppearance(): AppearancePreset {
    return {
      id: 'ocean',
      name: 'Ocean Blue',
      description: 'Clean, bright theme for professional booking pages.',
      icon: 'bi-droplet-half',
      fontFamily: 'Inter',
      primaryColor: '#3b82f6',
      accentColor: '#0ea5e9',
      backgroundColor: '#f1f5f9',
      surfaceColor: '#ffffff',
      textColor: '#0f172a',
      secondaryColor: '#64748b',
      bannerImageUrl: ''
    };
  }
}