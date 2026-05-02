import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { VenueDashboardComponent } from './pages/venue-dashboard/venue-dashboard.component';
import { AuditoriumDashboardComponent } from './pages/auditorium-dashboard/auditorium-dashboard.component';
import { VenueFormComponent } from './pages/venue-form/venue-form.component';
import { AuditoriumFormComponent } from './pages/auditorium-form/auditorium-form.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { LayoutMatrixEditorComponent } from './pages/layout-matrix-editor/layout-matrix-editor.component';
import { EventsPageComponent } from './pages/events-page/events-page.component';
import { OrganizerDashboardComponent } from './pages/organizer-dashboard/organizer-dashboard.component';
import { OrganizerLayoutComponent } from './pages/organizer-layout/organizer-layout.component';
import { EventOccurrenceFormComponent } from './pages/event-occurrence-form/event-occurrence-form.component';
import { EventFormComponent } from './pages/event-form/event-form.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { PublicBookingLayoutComponent } from './pages/public-booking-layout/public-booking-layout.component';
import { PublicBookingMapComponent } from './pages/public-booking-map/public-booking-map.component';
import { PublicBookingCheckoutComponent } from './pages/public-booking-checkout/public-booking-checkout.component';
import { PublicBookingSuccessComponent } from './pages/public-booking-success/public-booking-success.component';

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  { path: 'landingpage', component: LandingPageComponent },
  { path: 'login', component: LoginPageComponent},
  { path: 'register', component: RegisterPageComponent },

  {
    path: 'dashboard',
    component: OrganizerLayoutComponent,
    children: [
      { path: '', component: OrganizerDashboardComponent },
      { path: 'venues', component: VenueDashboardComponent },
      { path: 'venues/form', component: VenueFormComponent },
      { path: 'auditoriums/:venueId', component: AuditoriumDashboardComponent },
      { path: 'auditoriums/:venueId/form', component: AuditoriumFormComponent },
      { path: 'events', component: EventsPageComponent },
      { path: 'events/new', component: EventFormComponent },
      { path: 'events/:eventId/edit', component: EventFormComponent },
      { path: 'events/:eventId/occurrences/new', component: EventOccurrenceFormComponent },
      { path: 'events/:eventId/occurrences/:id/edit', component: EventOccurrenceFormComponent },
    ]
  },

  { path: 'checkout', component: CheckoutComponent },
  {
    path: 'book/:slug/:occurrenceId',
    component: PublicBookingLayoutComponent,
    children: [
      { path: '', component: PublicBookingMapComponent },
      { path: 'checkout', component: PublicBookingCheckoutComponent },
      { path: 'success', component: PublicBookingSuccessComponent }
    ]
  },
  { path: 'layout-matrix/:auditoriumId/editor', component: LayoutMatrixEditorComponent },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
