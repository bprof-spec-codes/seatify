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
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    AppComponent,
    LandingPageComponent,
    FooterComponent,
    SeatMapDisplayComponent,
    VenueDashboardComponent,
    ConfirmMessageComponent,
    AuditoriumDashboardComponent,
    LayoutMatrixEditorComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    CommonModule
  ],
  providers: [
    provideHttpClient(withInterceptors([]))
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
