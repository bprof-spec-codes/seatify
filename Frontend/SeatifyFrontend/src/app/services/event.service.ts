import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import EventRequest from '../models/event.request';
import { Observable } from 'rxjs';
import EventResponse from '../models/event.response';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor(private http: HttpClient) { }

  createEvent(eventrequest: EventRequest): Observable<EventResponse>{
    return this.http.post<EventResponse>("http://localhost:4200/api/event-occurrences", eventrequest);
  }

  updateEvent(eventrequest: EventRequest, id: number): Observable<EventResponse>{
    return this.http.put<EventResponse>("http://localhost:4200/update-event/" + id, eventrequest);
  }
}
