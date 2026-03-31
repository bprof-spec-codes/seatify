import { Component, OnDestroy, OnInit } from '@angular/core';
import { VenueService } from '../../services/venue.service';
import { Observable, of, Subject, takeUntil } from 'rxjs';
import { Venue } from '../../models/venue';
import { Router } from '@angular/router';

@Component({
  selector: 'app-venue-dashboard',
  standalone: false,
  templateUrl: './venue-dashboard.component.html',
  styleUrl: './venue-dashboard.component.sass'
})
export class VenueDashboardComponent implements OnInit, OnDestroy {
  venues$!: Observable<Venue[]>;
  venues!: Venue[];
  showModal: boolean = false;
  selectedVenue!: Venue;
  organizerId: string = "org-id-01"; // mock data
  isVenuesCalled!: boolean;
  private unsubscribe$ = new Subject<void>();

  constructor(private venueService: VenueService, private router: Router) {}

  ngOnInit(): void {
    let fetchInitiated = false;

    this.venueService.venues$.pipe(takeUntil(this.unsubscribe$)).subscribe(venues => {
      // If venues already stored don't call the API again.
      if (venues.length > 0)
      {
        this.venues = venues;
        this.venues$ = of(venues);
        fetchInitiated = false;
      }
      // If venues is empty call the API once for the venues.
      else if (venues.length === 0 && !fetchInitiated)
      {
        fetchInitiated = true;
        this.fetchVenues();
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  showVenueAuditoriums(venue: Venue): void {
    this.router.navigate(['/auditoriums', venue.id]);
  }

  createVenue(): void {
    this.venueService.setEditMode(false);
    this.router.navigate(['/venues/form']);
  }

  editVenue(venue: Venue): void {
    this.venueService.setEditVenue(venue);
    this.venueService.setEditMode(true);
    this.router.navigate(['/venues/form']);
  }

  confirmDelete(venue: any): void {
    this.selectedVenue = venue;
    this.showModal = true;
  }

  deleteVenue(venue: Venue): void {
    this.venueService.deleteVenueById(venue.id!).subscribe({
      next: () => {
        console.log('Venue successfully deleted!');
        this.venues = this.venues.filter(v => v.id !== venue.id);
        this.venues$ = of(this.venues);
      },
      error: err => console.error('Error: ', err.message)
    });

    this.cancelDelete();
  }

  cancelDelete(): void {
    this.showModal = false;
  }

  private fetchVenues(): void {
    this.venueService.getVenuesByOrganizerId(this.organizerId).pipe(takeUntil(this.unsubscribe$)).subscribe(fetchedVenues => {
      this.venues = fetchedVenues;
      this.venues$ = of(fetchedVenues);
      this.isVenuesCalled = true;
    });
  }
}
