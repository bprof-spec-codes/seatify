import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { PublicBookingStateService, SelectedSeat } from '../../services/public-booking-state.service';
import { ReservationService } from '../../services/reservation.service';
import { EventService } from '../../services/event.service';
import { BookingSessionService } from '../../services/booking-session.service';
import { BookingSession } from '../../models/booking.model';

@Component({
  selector: 'app-public-booking-checkout',
  standalone: false,
  templateUrl: './public-booking-checkout.component.html',
  styleUrls: ['./public-booking-checkout.component.sass']
})
export class PublicBookingCheckoutComponent implements OnInit, OnDestroy {
  checkoutForm: FormGroup;
  occ: any = null;
  session: BookingSession | null = null;
  selectedSeats: SelectedSeat[] = [];
  totalPrice = 0;
  currency = 'EUR';

  isLoading = true;
  isSubmitting = false;
  errorMessage = '';

  private sub = new Subscription();

  constructor(
    private fb: FormBuilder,
    private stateService: PublicBookingStateService,
    private reservationService: ReservationService,
    private eventService: EventService,
    private bookingSessionService: BookingSessionService,
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
    this.isLoading = true;
    const sessionId = this.resolveSessionId();
    if (!sessionId) {
      this.isLoading = false;
      this.errorMessage = 'Missing booking session. Please start a new booking.';
      return;
    }

    this.sub.add(
      this.bookingSessionService.getActiveSession(sessionId).subscribe({
        next: (session) => {
          if (this.isSessionExpired(session)) {
            this.handleExpiredSession();
            return;
          }

          this.session = session;
          this.stateService.setBookingSessionId(session.id);
          this.selectedSeats = session.holds.map(hold => ({
            seatId: hold.seatId,
            seatLabel: hold.seatLabel || hold.seatId,
            price: hold.basePrice
          }));
          this.totalPrice = this.selectedSeats.reduce((sum, seat) => sum + seat.price, 0);

          this.loadOccurrence(session.eventOccurrenceId);
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Failed to load booking session.';
        }
      })
    );
  }

  private resolveSessionId(): string | null {
    return this.route.snapshot.queryParamMap.get('sessionId')
      || this.route.snapshot.queryParamMap.get('bookingSessionId')
      || this.stateService.getBookingSessionId();
  }

  private loadOccurrence(occurrenceId: string): void {
    this.sub.add(
      this.eventService.getOccurrenceById(occurrenceId).subscribe({
        next: (occ) => {
          this.occ = occ;
          this.currency = occ.effectiveCurrency;
          this.stateService.setEventOccurrence(occ);
          this.persistCheckoutContext();
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
          this.errorMessage = 'Failed to load event details.';
        }
      })
    );
  }

  onSubmit(): void {
    if (this.checkoutForm.invalid || this.isSubmitting || !this.session) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const request = {
      eventOccurrenceId: this.session.eventOccurrenceId,
      customerName: this.checkoutForm.value.fullName || null,
      customerEmail: this.checkoutForm.value.email,
      customerPhone: this.checkoutForm.value.phone || null,
      seatIds: this.session.holds.map(h => h.seatId),
      bookingSessionId: this.session.id
    };

    this.sub.add(
      this.reservationService.checkoutReservation(request).subscribe({
        next: (res) => {
          this.isSubmitting = false;
          this.persistCheckoutResult(res);
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

  private isSessionExpired(session: BookingSession): boolean {
    return session.status !== 'Active' || session.phase === 'Expired' || session.phase === 'Cancelled';
  }

  private handleExpiredSession(): void {
    this.isLoading = false;
    this.errorMessage = 'Your booking session has expired. Please select your seats again.';
    this.stateService.setBookingSessionId(null);
    this.stateService.setSelectedSeats([]);
    window.setTimeout(() => {
      this.router.navigate(['../'], { relativeTo: this.route });
    }, 2500);
  }

  private persistCheckoutResult(result: any): void {
    try {
      window.sessionStorage.setItem('seatify.lastCheckoutResult', JSON.stringify(result));
    } catch {
      // ignore storage failures
    }
  }

  private persistCheckoutContext(): void {
    if (!this.occ) return;

    try {
      window.sessionStorage.setItem('seatify.lastBookingOccurrence', JSON.stringify(this.occ));
    } catch {
      // ignore storage failures
    }
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
