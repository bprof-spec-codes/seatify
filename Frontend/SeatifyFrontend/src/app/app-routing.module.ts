import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { VenueDashboardComponent } from './pages/venue-dashboard/venue-dashboard.component';
import { AuditoriumDashboardComponent } from './pages/auditorium-dashboard/auditorium-dashboard.component';
import { VenueFormComponent } from './pages/venue-form/venue-form.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { EventsPageComponent } from './pages/events-page/events-page.component';
import { OrganizerDashboardComponent } from './pages/organizer-dashboard/organizer-dashboard.component';
import { OrganizerLayoutComponent } from './pages/organizer-layout/organizer-layout.component';

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  { path: 'landingpage', component: LandingPageComponent },

  {
    path: 'dashboard',
    component: OrganizerLayoutComponent,
    children: [
      { path: '', component: OrganizerDashboardComponent },
      { path: 'venues', component: VenueDashboardComponent },
      { path: 'venues/form', component: VenueFormComponent },
      { path: 'auditoriums/:venueId', component: AuditoriumDashboardComponent },
      { path: 'events', component: EventsPageComponent }
    ]
  },

  { path: 'checkout', component: CheckoutComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
