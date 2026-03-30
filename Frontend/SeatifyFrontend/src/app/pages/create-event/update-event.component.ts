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
  //console.log("working", data)
  const eventRequest: EventRequest = {
    name: data.name,
  description: data.description,
  startsAt: data.startsAt,
  endsAt: data.endsAt,
  basePrice: data.basePrice,
  }
  this.eventService.updateEvent(eventRequest, eventId).subscribe(resp => alert("succesful save"))
 }
}
