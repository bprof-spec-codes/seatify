import { Component, OnInit } from '@angular/core';
import { VenueService } from '../../services/venue.service';
import { Observable } from 'rxjs';
import { Venue } from '../../models/venue';

@Component({
  selector: 'app-venue-dashboard',
  standalone: false,
  templateUrl: './venue-dashboard.component.html',
  styleUrl: './venue-dashboard.component.sass'
})
export class VenueDashboardComponent implements OnInit {
  venues$!: Observable<Venue[]>;

  constructor(private venueService: VenueService) {}

  ngOnInit(): void {
    // TODO: replace it with the logged in organizer's Venues
    this.venues$ = this.venueService.getVenues();
  }
}
