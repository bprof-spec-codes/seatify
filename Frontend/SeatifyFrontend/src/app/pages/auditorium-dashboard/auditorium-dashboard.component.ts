import { Component, OnDestroy, OnInit } from '@angular/core';
import { Auditorium } from '../../models/auditorium';
import { ActivatedRoute, Router } from '@angular/router';
import { AuditoriumService } from '../../services/auditorium.service';
import { Observable, of, Subject, takeUntil } from 'rxjs';
import { Venue } from '../../models/venue';
import { VenueService } from '../../services/venue.service';

@Component({
  selector: 'app-auditorium-dashboard',
  standalone: false,
  templateUrl: './auditorium-dashboard.component.html',
  styleUrl: './auditorium-dashboard.component.sass'
})
export class AuditoriumDashboardComponent implements OnInit, OnDestroy {
  venue!: Venue;
  venue$!: Observable<Venue>;
  auditoriums$!: Observable<Auditorium[]>;
  auditoriums!: Auditorium[];
  showModal: boolean = false;
  selectedAuditorium!: Auditorium;
  isEmpty: boolean = false;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private router: Router, 
    private activatedRoute: ActivatedRoute, 
    private auditoriumService: AuditoriumService,
    private venueService: VenueService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      const venueId = params['venueId'];

      this.venueService.venues$.pipe(takeUntil(this.unsubscribe$)).subscribe(venues => {
        const foundVenue = venues.find(v => v.id === venueId);

        // If venues already stored don't call API again.
        if (foundVenue)
        {
          this.venue = foundVenue;
          this.venue$ = of(this.venue);
          this.auditoriums = this.venue.auditoriums;
          this.auditoriums$ = of(this.venue.auditoriums);
          this.isEmpty = this.auditoriums.length === 0;
        }
        // If venues is empty call the API for the venue by venueId.
        else
        {
          this.venueService.getVenueById(venueId).pipe(takeUntil(this.unsubscribe$)).subscribe(venue => {
            this.venue = venue;
            this.venue$ = of(this.venue);
            this.auditoriums = venue.auditoriums;
            this.auditoriums$ = of(this.auditoriums);
            this.isEmpty = this.auditoriums.length === 0;
          });
          /*this.auditoriumService.getAuditoriumsByVenueId(venueId).pipe(takeUntil(this.unsubscribe$)).subscribe(auditoriums => {
            this.auditoriums = auditoriums;
            this.auditoriums$ = of(this.auditoriums);
          });*/
        }
      });
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  back(): void {
    this.router.navigate(['dashboard/venues']);
  }

  viewAuditorium(auditorium: Auditorium): void {
    console.log('View auditorium! ', auditorium);
  }

  createAuditorium(): void {
    this.auditoriumService.setEditMode(false);
    this.router.navigate([`dashboard/auditoriums/${this.venue.id}/form`]);
  }

  editAuditorium(auditorium: Auditorium): void {
    this.auditoriumService.setEditMode(true);
    this.auditoriumService.setEditAuditorium(auditorium);
    this.router.navigate([`dashboard/auditoriums/${this.venue.id}/form`]);
  }

  confirmDelete(auditorium: Auditorium): void {
    this.selectedAuditorium = auditorium;
    this.showModal = true;
  }

  deleteAuditorium(auditorium: Auditorium): void {
    this.auditoriumService.deleteAuditoriumById(auditorium.id!).subscribe({
      next: () => {
        console.log('Auditorium successfully deleted!');

        this.auditoriums = this.auditoriums.filter(a => a.id !== auditorium.id);
        this.auditoriums$ = of(this.auditoriums);

        // Update the local state to reflect the changes without unnecessary API calls.
        this.venueService.removeAuditoriumFromVenue(this.venue.id!, auditorium.id!);
      },
      error: err => console.error('Error: ', err.message)
    });

    this.cancelDelete();
  }

  cancelDelete(): void {
    this.showModal = false;
  }
}
