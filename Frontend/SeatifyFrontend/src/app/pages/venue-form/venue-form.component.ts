import { Component, OnDestroy, OnInit } from '@angular/core';
import { Venue } from '../../models/venue';
import { Observable, of, Subject, takeUntil } from 'rxjs';
import { VenueService } from '../../services/venue.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-venue-form',
  standalone: false,
  templateUrl: './venue-form.component.html',
  styleUrl: './venue-form.component.sass'
})
export class VenueFormComponent implements OnInit, OnDestroy {
  venueForm!: FormGroup;
  venue!: Venue;
  venue$!: Observable<Venue>;
  editMode!: boolean;
  editMode$!: Observable<boolean>;
  organizerId: string = "org-id-01"; // mock data
  private unsubscribe$ = new Subject<void>();

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private venueService: VenueService
  ) {}

  ngOnInit(): void {
    this.venueForm = this.fb.group({
      venueName: ['', [Validators.required, Validators.maxLength(100)]],
      venueCity: ['', [Validators.required, Validators.maxLength(100)]],
      venuePostalCode: ['', [Validators.required, Validators.maxLength(100)]],
      venueAddressLine: ['', [Validators.required, Validators.maxLength(100)]]
    });

    let fetchInitiated = false;

    // If venues is empty call the API once for the venues.
    this.venueService.venues$.pipe(takeUntil(this.unsubscribe$)).subscribe(venues => {
      if (venues.length === 0 && !fetchInitiated)
      {
        fetchInitiated = true;
        this.venueService.loadVenuesByOrganizerId(this.organizerId);
      }
    });

    // Get venue for editing in editing mode.
    this.venueService.editMode$.pipe(takeUntil(this.unsubscribe$)).subscribe(editMode => {
      this.editMode = editMode;
      this.editMode$ = of(editMode);
      if (this.editMode)
      {
        this.fetchVenue();
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onSubmit() {
    if (this.venueForm.valid)
    {
      const venueData: Venue = {
        id: this.editMode ? this.venue.id : undefined, // Only include the ID when in editing mode.
        name: this.venueForm.value.venueName,
        city: this.venueForm.value.venueCity,
        postalCode: this.venueForm.value.venuePostalCode,
        addressLine: this.venueForm.value.venueAddressLine,
        auditoriums: [],
        organizerId: this.organizerId
      };

      if (this.editMode)
      {
        this.venueService.updateVenue(venueData).pipe(takeUntil(this.unsubscribe$)).subscribe(() => {
          this.router.navigate(['/venues']);
        });
      }
      else
      {
        this.venueService.createVenue(venueData).pipe(takeUntil(this.unsubscribe$)).subscribe(() => {
          this.router.navigate(['/venues']);
        });
      }
    }
  }

  back(): void {
    this.router.navigate(['/venues']);
  }

  private fetchVenue(): void {
    this.venueService.getEditVenue().pipe(takeUntil(this.unsubscribe$)).subscribe(venue => {
      this.venue = venue;
      this.venueForm.patchValue({
        venueName: this.venue.name,
        venueCity: this.venue.city,
        venuePostalCode: this.venue.postalCode,
        venueAddressLine: this.venue.addressLine
      });
    });
  }
}
