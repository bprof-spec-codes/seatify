import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EventFormComponent } from './eventform.component';
import { EventService } from '../../services/event.service';
import EventRequest from '../../models/event.request';

@Component({
  selector: 'app-create-event',
  standalone: true,
  imports: [
    EventFormComponent
  ],
  templateUrl: './create-event.component.html',
  styleUrl: './create-event.component.sass'
})
export class CreateEventComponent {
  constructor(private eventService: EventService){}

 onCreate(data: any){
  const eventRequest: EventRequest = {
      name: data.name,
      description: data.description,
      startsAt: data.startsAt,
      endsAt: data.endsAt,
      basePrice: data.basePrice,
      // Új adatok bekötése
      logoImageUrl: data.logoImageUrl,
      bannerImageUrl: data.bannerImageUrl,
      themePreset: data.themePreset,
      venueId: data.venueId,
      auditoriumId: data.auditoriumId
    };

    this.eventService.createEvent(eventRequest).subscribe({
      next: (resp) => alert("Event created successfully!"),
      error: (err) => alert("Failed to create event. Please try again.")
    });
 }
}
