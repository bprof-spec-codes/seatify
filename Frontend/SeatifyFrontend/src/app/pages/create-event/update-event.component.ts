import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EventFormComponent } from './eventform.component';
import { EventService } from '../../services/event.service';
import EventRequest from '../../models/event.request';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-update-event',
  standalone: true,
  imports: [
    EventFormComponent
  ],
  templateUrl: './update-event.component.html',
  //styleUrl: './update-event.component.sass'
})
export class UpdateEventComponent {
  constructor(private eventService: EventService, private route: ActivatedRoute){}

 onUpdate(data: any){
    const eventId: number = parseInt(this.route.snapshot.paramMap.get('id') || '0');
    
    const eventRequest: EventRequest = {
      name: data.name,
      description: data.description,
      startsAt: data.startsAt,
      endsAt: data.endsAt,
      basePrice: data.basePrice,
      logoImageUrl: data.logoImageUrl,
      bannerImageUrl: data.bannerImageUrl,
      themePreset: data.themePreset,
      venueId: data.venueId,
      auditoriumId: data.auditoriumId
    };

    this.eventService.updateEvent(eventRequest, eventId).subscribe({
      next: (resp) => alert("Event updated successfully!"),
      error: (err) => alert("Failed to update event. Please try again.")
    });
 }
}
