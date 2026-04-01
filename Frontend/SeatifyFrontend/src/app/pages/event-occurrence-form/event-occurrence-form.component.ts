import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { VenueService } from '../../services/venue.service';
import { AuditoriumService } from '../../services/auditorium.service';
import { Venue } from '../../models/venue';
import { Auditorium } from '../../models/auditorium';
import { SeatifyEvent } from '../../models/event';

@Component({
  selector: 'app-event-occurrence-form',
  standalone: false,
  templateUrl: './event-occurrence-form.component.html',
  styleUrl: './event-occurrence-form.component.sass'
})
export class EventOccurrenceFormComponent implements OnInit {
  occurrenceForm: FormGroup;
  eventId: string = '';
  occurrenceId: string | null = null;
  event: SeatifyEvent | null = null;
  venues: Venue[] = [];
  auditoriums: Auditorium[] = [];
  isEditMode = false;
  isLoading = true;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private venueService: VenueService,
    private auditoriumService: AuditoriumService
  ) {
    this.occurrenceForm = this.fb.group({
      venueId: ['', Validators.required],
      auditoriumId: ['', Validators.required],
      startsAt: ['', Validators.required],
      endsAt: ['', Validators.required],
      bookingOpenAt: ['', Validators.required],
      bookingCloseAt: ['', Validators.required],
      hasDoorsOpenTime: [false],
      doorsOpenAt: [{ value: '', disabled: true }]
    });
  }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('eventId') || '';
    this.occurrenceId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.occurrenceId;

    this.loadInitialData();
  }

  async loadInitialData() {
    this.isLoading = true;
    try {
      // 1. Load Event name
      this.eventService.getEventById(this.eventId).subscribe(ev => this.event = ev);

      // 2. Load Venues
      this.venueService.getVenuesByOrganizerId('placeholder-organizer-id').subscribe(vn => {
        this.venues = vn;
        
        // 3. If Edit Mode, load occurrence and patch
        if (this.isEditMode && this.occurrenceId) {
          this.eventService.getOccurrenceById(this.occurrenceId).subscribe(occ => {
            this.onVenueChange(occ.venueId, false); // Load auditoriums for the venue
            
            this.occurrenceForm.patchValue({
              venueId: occ.venueId,
              auditoriumId: occ.auditoriumId,
              startsAt: this.formatDateForInput(occ.startsAtUtc),
              endsAt: this.formatDateForInput(occ.endsAtUtc),
              bookingOpenAt: this.formatDateForInput(occ.bookingOpenAtUtc),
              bookingCloseAt: this.formatDateForInput(occ.bookingCloseAtUtc),
              hasDoorsOpenTime: !!occ.doorsOpenAtUtc,
              doorsOpenAt: occ.doorsOpenAtUtc ? this.formatDateForInput(occ.doorsOpenAtUtc) : ''
            });

            if (occ.doorsOpenAtUtc) {
              this.occurrenceForm.get('doorsOpenAt')?.enable();
            }
          });
        }
      });
    } catch (error) {
      console.error('Failed to load form data', error);
    } finally {
      this.isLoading = false;
    }
  }

  onVenueChange(venueId: string, resetAuditorium = true) {
    if (resetAuditorium) {
      this.occurrenceForm.patchValue({ auditoriumId: '' });
    }
    
    if (venueId) {
      this.auditoriumService.getAuditoriumsByVenueId(venueId).subscribe(auds => {
        this.auditoriums = auds;
      });
    } else {
      this.auditoriums = [];
    }
  }

  toggleDoorsOpen() {
    const hasDoorsOpen = this.occurrenceForm.get('hasDoorsOpenTime')?.value;
    const ctrl = this.occurrenceForm.get('doorsOpenAt');
    if (hasDoorsOpen) {
      ctrl?.enable();
    } else {
      ctrl?.disable();
      ctrl?.setValue('');
    }
  }

  formatDateForInput(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    return date.toISOString().slice(0, 16);
  }

  onSubmit() {
    if (this.occurrenceForm.invalid) {
      this.occurrenceForm.markAllAsTouched();
      return;
    }

    const val = this.occurrenceForm.getRawValue();
    const payload = {
      eventId: this.eventId,
      venueId: val.venueId,
      auditoriumId: val.auditoriumId,
      startsAtUtc: new Date(val.startsAt).toISOString(),
      endsAtUtc: new Date(val.endsAt).toISOString(),
      bookingOpenAtUtc: new Date(val.bookingOpenAt).toISOString(),
      bookingCloseAtUtc: new Date(val.bookingCloseAt).toISOString(),
      doorsOpenAtUtc: val.hasDoorsOpenTime && val.doorsOpenAt ? new Date(val.doorsOpenAt).toISOString() : null,
      status: 'Published'
    };

    if (this.isEditMode && this.occurrenceId) {
      this.eventService.updateOccurrence(this.occurrenceId, payload).subscribe({
        next: () => this.router.navigate(['/dashboard/events']),
        error: (err) => alert('Failed to update occurrence')
      });
    } else {
      this.eventService.createOccurrence(payload).subscribe({
        next: () => this.router.navigate(['/dashboard/events']),
        error: (err) => alert('Failed to create occurrence')
      });
    }
  }

  cancel() {
    this.router.navigate(['/dashboard/events']);
  }
}
