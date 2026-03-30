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
  //console.log("working", data)
  const eventRequest: EventRequest = {
    name: data.name,
  description: data.description,
  startsAt: data.startsAt,
  endsAt: data.endsAt,
  basePrice: data.basePrice,
  }
  this.eventService.createEvent(eventRequest).subscribe(resp => alert("succesful save"))
 }
}
