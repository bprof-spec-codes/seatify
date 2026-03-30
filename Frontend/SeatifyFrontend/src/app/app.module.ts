import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { FooterComponent } from './pages/footer/footer.component';
import { SeatMapDisplayComponent } from './helpers/seat-map-display/seat-map-display.component';
import { CreateEventComponent } from './pages/create-event/create-event.component';
import { EventFormComponent } from './pages/create-event/eventform.component';
import { provideHttpClient } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    LandingPageComponent,
    FooterComponent,
    SeatMapDisplayComponent,
    //CreateEventComponent,
    //EventFormComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [provideHttpClient()],
  bootstrap: [AppComponent]
})
export class AppModule { }
