import { Component } from '@angular/core';
import { VenueDashboardComponent } from '../venue-dashboard/venue-dashboard.component';
import { EventsPageComponent } from '../events-page/events-page.component';

type OrganizerPage = 'dashboard' | 'venues' | 'events';

@Component({
  selector: 'app-organizer-dashboard',
  standalone: false,
  templateUrl: './organizer-dashboard.component.html',
  styleUrls: ['./organizer-dashboard.component.sass']
})
export class OrganizerDashboardComponent {
  activePage: OrganizerPage = 'dashboard';

  readonly venueDashboardComponent = VenueDashboardComponent;
  readonly eventsPageComponent = EventsPageComponent;

  setActivePage(page: OrganizerPage): void {
    this.activePage = page;
  }

  isActive(page: OrganizerPage): boolean {
    return this.activePage === page;
  }

  

  getPageTitle(): string {
    switch (this.activePage) {
      case 'venues':
        return 'Venues';
      case 'events':
        return 'Events';
      default:
        return 'Dashboard';
    }
  }

  getPageSubtitle(): string {
    switch (this.activePage) {
      case 'venues':
        return 'Manage your venues and auditoriums';
      case 'events':
        return 'Manage your event catalog and schedules';
      default:
        return 'Overview of your ticketing platform';
    }
  }
}