import { Component, OnDestroy, OnInit } from '@angular/core';
import { VenueService } from '../../services/venue.service';
import { distinctUntilChanged, map, Observable, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { Venue } from '../../models/venue';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

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
  organizerId: string | null = "";
  isVenuesCalled!: boolean;
  private unsubscribe$ = new Subject<void>();

  constructor(private venueService: VenueService, private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$
    .pipe(
      takeUntil(this.unsubscribe$),
      map(user => user?.organizerId ?? null),
      distinctUntilChanged(),
      tap(organizerId => {
        this.organizerId = organizerId;
        this.venues = [];
        this.venues$ = of([]);
        this.isVenuesCalled = false;
      }),
      switchMap(organizerId => {
        if (!organizerId) return of([] as Venue[]);
        this.fetchVenues();
        return this.venueService.venues$;
      })
    )
    .subscribe(venues => {
      if (!venues) return;
      this.venues = venues;
      this.venues$ = of(venues);
      this.isVenuesCalled = venues.length > 0;
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  showVenueAuditoriums(venue: Venue): void {
    this.router.navigate(['dashboard/auditoriums', venue.id]);
  }

  createVenue(): void {
    this.venueService.setEditMode(false);
    this.router.navigate(['dashboard/venues/form']);
  }

  editVenue(venue: Venue): void {
    this.venueService.setEditVenue(venue);
    this.venueService.setEditMode(true);
    this.router.navigate(['dashboard/venues/form']);
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
    this.venueService.getVenuesByOrganizerId(this.organizerId ?? "").pipe(takeUntil(this.unsubscribe$)).subscribe(fetchedVenues => {
      this.venues = fetchedVenues;
      this.venues$ = of(fetchedVenues);
      this.isVenuesCalled = true;
    });
  }
}
