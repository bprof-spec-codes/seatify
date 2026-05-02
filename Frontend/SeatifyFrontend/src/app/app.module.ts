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
import { LayoutMatrixEditorComponent } from './pages/layout-matrix-editor/layout-matrix-editor.component';
import { CommonModule, DecimalPipe } from '@angular/common';
import { LayoutMatrixFormComponent } from './pages/layout-matrix-form/layout-matrix-form.component';
import { SeatEditorComponent } from './pages/seat-editor/seat-editor.component';
import { VenueFormComponent } from './pages/venue-form/venue-form.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { SectorEditorComponent } from './pages/sector-editor/sector-editor.component';
import { SectorFormComponent } from './pages/sector-form/sector-form.component';
import { EventsPageComponent } from './pages/events-page/events-page.component';
import { OrganizerLayoutComponent } from './pages/organizer-layout/organizer-layout.component';
import { OrganizerDashboardComponent } from './pages/organizer-dashboard/organizer-dashboard.component';
import { AuditoriumFormComponent } from './pages/auditorium-form/auditorium-form.component';
import { EventOccurrenceFormComponent } from './pages/event-occurrence-form/event-occurrence-form.component';
import { EventFormComponent } from './pages/event-form/event-form.component';
import { CheckinModalComponent } from './components/checkin-modal/checkin-modal.component';
import { authInterceptor } from './interceptors/auth.interceptor';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { PublicBookingLayoutComponent } from './pages/public-booking-layout/public-booking-layout.component';
import { PublicBookingMapComponent } from './pages/public-booking-map/public-booking-map.component';
import { PublicBookingCheckoutComponent } from './pages/public-booking-checkout/public-booking-checkout.component';
import { PublicBookingSuccessComponent } from './pages/public-booking-success/public-booking-success.component';

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
    LayoutMatrixEditorComponent,
    LayoutMatrixFormComponent,
    SeatEditorComponent,
    SectorEditorComponent,
    SectorFormComponent,
    EventsPageComponent,
    OrganizerLayoutComponent,
    OrganizerDashboardComponent,
    AuditoriumFormComponent,
    EventOccurrenceFormComponent,
    EventFormComponent,
    CheckinModalComponent,
    LoginPageComponent,
    RegisterPageComponent,
    PublicBookingLayoutComponent,
    PublicBookingMapComponent,
    PublicBookingCheckoutComponent,
    PublicBookingSuccessComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    CommonModule,
    DecimalPipe,
    FormsModule,
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor]))
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
