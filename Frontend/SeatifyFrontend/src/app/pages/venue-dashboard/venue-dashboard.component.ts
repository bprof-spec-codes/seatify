import { Component, OnInit } from '@angular/core';
import { VenueService } from '../../services/venue.service';
import { Observable } from 'rxjs';
import { Venue } from '../../models/venue';
import { Auditorium } from '../../models/auditorium';

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

  showVenueAuditoriums(auditoriums: Auditorium[]): void {
    console.log('Auditoriums: ', auditoriums);
  }

  createVenue(): void {
    console.log('Create venue!');
  }

  editVenue(venue: Venue): void {
    console.log('Editing venue: ', venue);
  }

  deleteVenue(venue: Venue): void {
    console.log('Deleting venue: ', venue);
  }
}
