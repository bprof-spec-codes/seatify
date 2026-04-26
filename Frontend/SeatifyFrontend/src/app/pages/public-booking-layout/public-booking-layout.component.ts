import { Component, OnInit, Renderer2 } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EventService } from '../../services/event.service';
import { PublicBookingStateService } from '../../services/public-booking-state.service';
import { EventOccurrence } from '../../models/event-occurrence';

@Component({
  selector: 'app-public-booking-layout',
  standalone: false,
  template: `
    <div class="public-layout" [style.--primary-color]="primaryColor" [style.--secondary-color]="secondaryColor">
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
      background: #f8f9fa
      display: flex
      flex-direction: column

    .public-nav
      background: #ffffff
      padding: 1rem 0
      box-shadow: 0 2px 10px rgba(0,0,0,0.05)
      border-top: 4px solid var(--primary-color, #0984e3)

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
        color: #2d3436
      .event-date
        font-size: 0.85rem
        color: #636e72

    main
      flex: 1

    .public-footer
      font-size: 0.9rem
  `]
})
export class PublicBookingLayoutComponent implements OnInit {
  occurrence: EventOccurrence | null = null;
  primaryColor = '#0984e3';
  secondaryColor = '#74b9ff';
  logoUrl = '';
  currentYear = new Date().getFullYear();

  constructor(
    private route: ActivatedRoute,
    private eventService: EventService,
    private stateService: PublicBookingStateService,
    private renderer: Renderer2
  ) {}

  ngOnInit(): void {
    const occurrenceId = this.route.snapshot.paramMap.get('occurrenceId');
    if (occurrenceId) {
      this.eventService.getOccurrenceById(occurrenceId).subscribe((occ: EventOccurrence) => {
        this.occurrence = occ;
        this.stateService.setEventOccurrence(occ);
        
        if (occ.event?.primaryColor) this.primaryColor = occ.event.primaryColor;
        if (occ.event?.secondaryColor) this.secondaryColor = occ.event.secondaryColor;
        if (occ.event?.logoImageUrl) this.logoUrl = occ.event.logoImageUrl;

        this.applyTheme();
      });
    }
  }

  private applyTheme() {
    const host = document.documentElement;
    host.style.setProperty('--primary-color', this.primaryColor);
    host.style.setProperty('--secondary-color', this.secondaryColor);
  }
}
