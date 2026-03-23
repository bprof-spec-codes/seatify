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

@NgModule({
  declarations: [
    AppComponent,
    LandingPageComponent,
    FooterComponent,
    SeatMapDisplayComponent,
    VenueDashboardComponent,
    ConfirmMessageComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [
    provideHttpClient(withInterceptors([]))
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
