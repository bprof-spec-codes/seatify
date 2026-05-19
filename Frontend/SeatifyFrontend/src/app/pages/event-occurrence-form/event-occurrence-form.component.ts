import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import { VenueService } from '../../services/venue.service';
import { AuditoriumService } from '../../services/auditorium.service';
import { AppearanceService } from '../../services/appearance.service';
import { Appearance } from '../../models/appearance';
import { Venue } from '../../models/venue';
import { Auditorium } from '../../models/auditorium';
import { SeatifyEvent } from '../../models/event';
import { AuthService } from '../../services/auth.service';
import { filter } from 'rxjs';
import { EventOccurrence } from '../../models/event-occurrence';

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
  hasBookings = false;
  appearances: Appearance[] = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService,
    private venueService: VenueService,
    private auditoriumService: AuditoriumService,
    private authService: AuthService,
    private appearanceService: AppearanceService
  ) {
    this.occurrenceForm = this.fb.group({
      venueId: ['', Validators.required],
      auditoriumId: ['', Validators.required],
      startsAt: ['', Validators.required],
      endsAt: ['', Validators.required],
      bookingOpenAt: ['', Validators.required],
      bookingCloseAt: ['', Validators.required],
      hasDoorsOpenTime: [false],
      doorsOpenAt: [{ value: '', disabled: true }],
      currencyOverride: [''],
      appearanceId: [null]
    });
  }

  get inheritedCurrency(): string {
    if (this.event?.currency) return this.event.currency;
    
    const selectedAudId = this.occurrenceForm.get('auditoriumId')?.value;
    const selectedAud = this.auditoriums.find(a => a.id === selectedAudId);
    if (selectedAud?.currency) return selectedAud.currency;
    
    return 'EUR';
  }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('eventId') || '';
    this.occurrenceId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.occurrenceId;

    this.authService.currentUser$
      .pipe(filter((u): u is NonNullable<typeof u> => !!u))
      .subscribe(user => this.loadInitialData(user.organizerId));
  }

  async loadInitialData(organizerId: any) {
    this.isLoading = true;
    try {
      // 1. Load Event name
      this.eventService.getEventById(this.eventId).subscribe(ev => this.event = ev);

      this.appearanceService.getMyAppearances().subscribe(apps => {
        console.log('Appearances loaded in OccForm:', apps);
        this.appearances = apps;
      });

      // 2. Load Venues
      this.venueService.getVenuesByOrganizerId(organizerId).subscribe(vn => {
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
              doorsOpenAt: occ.doorsOpenAtUtc ? this.formatDateForInput(occ.doorsOpenAtUtc) : '',
              currencyOverride: occ.currencyOverride || '',
              appearanceId: occ.appearanceId || null
            });

            if (occ.doorsOpenAtUtc) {
              this.occurrenceForm.get('doorsOpenAt')?.enable();
            }

            if (occ.hasBookings) {
              this.hasBookings = true;
              this.occurrenceForm.get('venueId')?.disable();
              this.occurrenceForm.get('auditoriumId')?.disable();
              this.occurrenceForm.get('startsAt')?.disable();
              this.occurrenceForm.get('endsAt')?.disable();
              this.occurrenceForm.get('currencyOverride')?.disable();
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

  toUtcIso(datetimeLocal?: string): string | undefined {
    if (!datetimeLocal) return undefined;
    const [date, time = '00:00:00'] = datetimeLocal.split('T');
    const [y, m, d] = date.split('-').map(Number);
    const [hh, mm, ss = '00'] = time.split(':');
    return new Date(Date.UTC(y, m - 1, Number(d), Number(hh), Number(mm), Number(ss))).toISOString();
  }

  onSubmit() {
    if (this.occurrenceForm.invalid) {
      this.occurrenceForm.markAllAsTouched();
      return;
    }

    const val = this.occurrenceForm.getRawValue();
    const payload: Partial<EventOccurrence> = {
      eventId: this.eventId,
      venueId: val.venueId,
      auditoriumId: val.auditoriumId,
      startsAtUtc: this.toUtcIso(val.startsAt),
      endsAtUtc: this.toUtcIso(val.endsAt),
      bookingOpenAtUtc: this.toUtcIso(val.bookingOpenAt),
      bookingCloseAtUtc: this.toUtcIso(val.bookingCloseAt),
      doorsOpenAtUtc: val.hasDoorsOpenTime ? this.toUtcIso(val.doorsOpenAt) : undefined,
      currencyOverride: val.currencyOverride || undefined,
      appearanceId: val.appearanceId || undefined,
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
