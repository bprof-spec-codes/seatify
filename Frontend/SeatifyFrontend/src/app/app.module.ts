import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { FooterComponent } from './pages/footer/footer.component';
import { SeatMapDisplayComponent } from './helpers/seat-map-display/seat-map-display.component';
import { VenueDashboardComponent } from './pages/venue-dashboard/venue-dashboard.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ConfirmMessageComponent } from './helpers/confirm-message/confirm-message.component';
import { AuditoriumDashboardComponent } from './pages/auditorium-dashboard/auditorium-dashboard.component';
import { VenueFormComponent } from './pages/venue-form/venue-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { EventsPageComponent } from './pages/events-page/events-page.component';
import { OrganizerLayoutComponent } from './pages/organizer-layout/organizer-layout.component';
import { OrganizerDashboardComponent } from './pages/organizer-dashboard/organizer-dashboard.component';
import { AuditoriumFormComponent } from './pages/auditorium-form/auditorium-form.component';

@NgModule({
  declarations: [
    AppComponent,
    LandingPageComponent,
    FooterComponent,
    SeatMapDisplayComponent,
    VenueDashboardComponent,
    ConfirmMessageComponent,
    AuditoriumDashboardComponent,
    VenueFormComponent,
    CheckoutComponent,
    EventsPageComponent,
    OrganizerLayoutComponent,
    OrganizerDashboardComponent,
    AuditoriumFormComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    provideHttpClient(withInterceptors([]))
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
