import { Component, OnInit, Renderer2 } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EventService } from '../../services/event.service';
import { AppearanceService } from '../../services/appearance.service';
import { PublicBookingStateService } from '../../services/public-booking-state.service';
import { EventOccurrence } from '../../models/event-occurrence';

@Component({
  selector: 'app-public-booking-layout',
  standalone: false,
  template: `
    <div class="public-layout" [style.--font-family]="fontFamily" [style.--primary-color]="primaryColor" [style.--secondary-color]="secondaryColor" [style.--bg-color]="backgroundColor" [style.--text-color]="textColor" [style.--surface-color]="surfaceColor" [style.--accent-color]="accentColor">
      <nav class="public-nav">
        <div class="container d-flex justify-content-between align-items-center">
          <div class="brand">
            <img *ngIf="logoUrl" [src]="logoUrl" alt="Logo" class="event-logo">
            <span class="system-name" *ngIf="!logoUrl">Seatify</span>
          </div>
          <div class="event-brief" *ngIf="occurrence">
            <span class="event-title">{{ occurrence?.event?.name }}</span>
            <span class="event-date">{{ occurrence?.startsAtUtc | date:'medium' }}</span>
          </div>
        </div>
      </nav>

      <div class="event-banner" *ngIf="bannerUrl" [style.background-image]="'url(' + bannerUrl + ')'"></div>

      <main class="container py-5" *ngIf="occurrence">
        <router-outlet></router-outlet>
      </main>

      <footer class="public-footer text-center py-4 text-muted">
        <p>&copy; {{ currentYear }} Powered by Seatify. All rights reserved.</p>
      </footer>
    </div>
  `,
  styles: [`
    .public-layout
      min-height: 100vh
      background: var(--bg-color, #f8f9fa)
      color: var(--text-color, #2d3436)
      font-family: var(--font-family, 'Inter')
      display: flex
      flex-direction: column

    .public-nav
      background: var(--surface-color, #ffffff)
      padding: 1rem 0
      box-shadow: 0 2px 10px rgba(0,0,0,0.05)
      border-top: 4px solid var(--primary-color, #0984e3)

    .event-banner
      height: 200px
      background-size: cover
      background-position: center
      border-bottom: 1px solid rgba(0,0,0,0.1)

    .brand
      .event-logo
        max-height: 40px
      .system-name
        font-weight: 900
        font-size: 1.5rem
        letter-spacing: -1px
        color: var(--primary-color, #0984e3)

    .event-brief
      display: flex
      flex-direction: column
      align-items: flex-end
      .event-title
        font-weight: 700
        color: var(--primary-color, #0984e3)
      .event-date
        font-size: 0.85rem
        color: color-mix(in srgb, var(--text-color) 70%, transparent)

    main
      flex: 1

    .public-footer
      font-size: 0.9rem
      color: color-mix(in srgb, var(--text-color) 60%, transparent)
  `]
})
export class PublicBookingLayoutComponent implements OnInit {
  occurrence: EventOccurrence | null = null;
  fontFamily = 'Inter';
  primaryColor = '#0984e3';
  secondaryColor = '#74b9ff';
  accentColor = '#0ea5e9';
  backgroundColor = '#f8f9fa';
  surfaceColor = '#ffffff';
  textColor = '#2d3436';
  logoUrl = '';
  bannerUrl = '';
  currentYear = new Date().getFullYear();

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private appearanceService: AppearanceService,
    private stateService: PublicBookingStateService,
    private renderer: Renderer2
  ) {}

  ngOnInit(): void {
    const occurrenceId = this.route.snapshot.paramMap.get('occurrenceId');
    const themeId = this.route.snapshot.queryParamMap.get('themeId');

    if (occurrenceId) {
      this.eventService.getOccurrenceById(occurrenceId).subscribe((occ: EventOccurrence) => {
        this.occurrence = occ;
        this.stateService.setEventOccurrence(occ);
        
        if (occ.event) {
          this.primaryColor = occ.event.primaryColor;
          this.secondaryColor = occ.event.secondaryColor;
          this.accentColor = occ.event.accentColor;
          this.backgroundColor = occ.event.backgroundColor;
          this.surfaceColor = occ.event.surfaceColor;
          this.textColor = occ.event.textColor;
          this.logoUrl = occ.event.logoImageUrl;
          this.bannerUrl = occ.event.bannerImageUrl;
          this.fontFamily = occ.event.fontFamily || 'Inter';
        }

        if (themeId) {
          this.appearanceService.getById(themeId).subscribe(theme => {
            this.fontFamily = theme.fontFamily || 'Inter';
            this.primaryColor = theme.primaryColor;
            this.secondaryColor = theme.secondaryColor;
            this.accentColor = theme.accentColor;
            this.backgroundColor = theme.backgroundColor;
            this.surfaceColor = theme.surfaceColor;
            this.textColor = theme.textColor;
            this.bannerUrl = theme.bannerImageUrl || this.bannerUrl;
            this.applyTheme();
          });
        } else {
          this.applyTheme();
        }
      });
    }
  }

  private applyTheme() {
    const host = document.documentElement;
    host.style.setProperty('--font-family', this.fontFamily);
    host.style.setProperty('--primary-color', this.primaryColor);
    host.style.setProperty('--secondary-color', this.secondaryColor);
    host.style.setProperty('--accent-color', this.accentColor);
    host.style.setProperty('--bg-color', this.backgroundColor);
    host.style.setProperty('--surface-color', this.surfaceColor);
    host.style.setProperty('--text-color', this.textColor);
  }
}
