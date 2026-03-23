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
  organizerId: string = "org-id-01"; // mock data

  constructor(private venueService: VenueService) {}

  ngOnInit(): void {
    this.venues$ = this.venueService.getVenuesByOrganizerId(this.organizerId);
  }
}
