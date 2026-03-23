import { Component, OnInit } from '@angular/core';
import { VenueService } from '../../services/venue.service';
import { Observable, of, Subject, takeUntil } from 'rxjs';
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
  venues!: Venue[];
  showModal: boolean = false;
  selectedVenue!: Venue;
  organizerId: string = "org-id-01"; // mock data
  private unsubscribe$ = new Subject<void>();

  constructor(private venueService: VenueService) {}

  ngOnInit(): void {
    this.venueService.getVenuesByOrganizerId(this.organizerId).pipe(takeUntil(this.unsubscribe$)).subscribe(venues => {
      this.venues = venues;
      this.venues$ = of(this.venues);
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
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

  confirmDelete(venue: any): void {
    this.selectedVenue = venue;
    this.showModal = true;
  }

  deleteVenue(venue: Venue): void {
    this.venueService.deleteVenueById(venue.id).subscribe({
      next: () => {
        console.log('Venue successfully deleted!');
        this.venues = this.venues.filter(v => v.id !== venue.id);
        this.venues$ = of(this.venues);
        //this.venues$ = this.venueService.getVenuesByOrganizerId(this.organizerId);
      },
      error: err => console.error('Error: ', err.message)
    });

    this.cancelDelete();
  }

  cancelDelete(): void {
    this.showModal = false;
  }
}
