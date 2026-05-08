import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { PublicBookingStateService, SelectedSeat } from '../../services/public-booking-state.service';
import { ReservationService } from '../../services/reservation.service';
import { EventService } from '../../services/event.service';

@Component({
  selector: 'app-public-booking-checkout',
  standalone: false,
  templateUrl: './public-booking-checkout.component.html',
  styleUrls: ['./public-booking-checkout.component.sass']
})
export class PublicBookingCheckoutComponent implements OnInit, OnDestroy {
  checkoutForm: FormGroup;
  occ: any = null;
  selectedSeats: SelectedSeat[] = [];
  totalPrice = 0;
  currency = 'EUR';

  isSubmitting = false;
  errorMessage = '';

  private sub = new Subscription();

  constructor(
    private fb: FormBuilder,
    private stateService: PublicBookingStateService,
    private reservationService: ReservationService,
    private eventService: EventService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.checkoutForm = this.fb.group({
      fullName: ['', [Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern('^\\+?[0-9\\s-]{7,15}$')]]
    });
  }

  ngOnInit(): void {
    this.occ = this.stateService.getEventOccurrence();
    this.selectedSeats = this.stateService.getSelectedSeats();
    this.totalPrice = this.stateService.getTotalPrice();

    if (this.occ) {
      this.currency = this.occ.effectiveCurrency;
    }

    if (!this.occ || this.selectedSeats.length === 0) {
      // If refreshed on checkout page, try to recover occurrence but seats are likely gone
      const occurrenceId = this.route.parent?.snapshot.paramMap.get('occurrenceId');
      if (occurrenceId && !this.occ) {
        this.eventService.getOccurrenceById(occurrenceId).subscribe(occ => {
          this.occ = occ;
          this.currency = occ.effectiveCurrency;
          this.stateService.setEventOccurrence(occ);
          if (this.selectedSeats.length === 0) {
             this.router.navigate(['../'], { relativeTo: this.route });
          }
        });
      } else {
        this.router.navigate(['../'], { relativeTo: this.route });
      }
    }
  }

  onSubmit(): void {
    if (this.checkoutForm.invalid || this.isSubmitting) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const request = {
      eventOccurrenceId: this.occ.id,
      customerName: this.checkoutForm.value.fullName || null,
      customerEmail: this.checkoutForm.value.email,
      customerPhone: this.checkoutForm.value.phone || null,
      seatIds: this.selectedSeats.map(s => s.seatId)
    };

    this.sub.add(
      this.reservationService.checkoutReservation(request).subscribe({
        next: (res) => {
          this.isSubmitting = false;
          (this.stateService as any).lastCheckoutResult = res;
          this.router.navigate(['../success'], { relativeTo: this.route });
        },
        error: (err) => {
          this.isSubmitting = false;
          this.errorMessage = err.error?.message || 'Checkout failed. Some seats may have been booked by someone else in the meantime.';
          console.error(err);
        }
      })
    );
  }

  goBack(): void {
    this.router.navigate(['../'], { relativeTo: this.route });
  }

  get f() { return this.checkoutForm.controls; }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
