import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EventService } from '../../services/event.service';
import EventRequest from '../../models/event.request';

@Component({
  selector: 'app-event-form',
  standalone: false,
  templateUrl: './event-form.component.html',
  styleUrl: './event-form.component.sass'
})
export class EventFormComponent implements OnInit {
  eventForm: FormGroup;
  eventId: string | null = null;
  isEditMode = false;
  isLoading = false;

  /** Unique currencies from all associated auditoriums */
  auditoriumCurrencies: string[] = [];

  /** Display string for inherited currency info */
  get inheritedCurrencyLabel(): string {
    if (this.auditoriumCurrencies.length === 0) return 'EUR (default)';
    const unique = [...new Set(this.auditoriumCurrencies)];
    return unique.join(' / ');
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private eventService: EventService
  ) {
    this.eventForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      slug: ['', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
      description: [''],
      status: ['Published', Validators.required],
      currency: ['']
    });
  }

  ngOnInit(): void {
    this.eventId = this.route.snapshot.paramMap.get('eventId');
    this.isEditMode = !!this.eventId;

    if (this.isEditMode && this.eventId) {
      this.loadEvent();
    }
  }

  loadEvent() {
    this.isLoading = true;
    this.eventService.getEventById(this.eventId!).subscribe({
      next: (event) => {
        this.eventForm.patchValue({
          name: event.name,
          slug: event.slug,
          description: event.description,
          status: event.status,
          currency: event.currency || ''
        });
        this.isLoading = false;

        // Load occurrences to determine the auditorium's inherited currency
        this.loadAuditoriumCurrency();
      },
      error: (err) => {
        console.error('Failed to load event', err);
        this.isLoading = false;
        alert('Failed to load event details.');
        this.router.navigate(['/dashboard/events']);
      }
    });
  }

  /** Loads all occurrences' auditoriums to show the full inherited currency picture */
  loadAuditoriumCurrency() {
    if (!this.eventId) return;

    this.eventService.getOccurrencesByEventId(this.eventId).subscribe({
      next: (occurrences) => {
        if (!occurrences || occurrences.length === 0) return;

        // Collect all unique auditorium currencies from all occurrences
        const currencies = occurrences
          .map(occ => occ.auditorium?.currency)
          .filter((c): c is string => !!c);

        this.auditoriumCurrencies = [...new Set(currencies)];
      },
      error: () => { /* keep empty = default EUR shown */ }
    });
  }

  onSubmit() {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const val = this.eventForm.value;
    const payload: EventRequest = {
      name: val.name,
      slug: val.slug,
      description: val.description || '',
      status: val.status,
      currency: val.currency || null,
      organizerId: 'org-id-01' // Matches seed data in backend
    };

    if (this.isEditMode && this.eventId) {
      // Itt a service-ben az updateEvent-nek number-t vár az id-ra, 
      // de a SeatifyEvent-ben string. A backend-en is string.
      // Kijavítom a service-t is.
      this.eventService.updateEvent(payload, this.eventId as any).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/dashboard/events']);
        },
        error: (err) => {
          this.isLoading = false;
          alert('Failed to update event: ' + (err.error?.message || err.message));
        }
      });
    } else {
      this.eventService.createEvent(payload).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/dashboard/events']);
        },
        error: (err) => {
          this.isLoading = false;
          alert('Failed to create event: ' + (err.error?.message || err.message));
        }
      });
    }
  }

  cancel() {
    this.router.navigate(['/dashboard/events']);
  }
}
