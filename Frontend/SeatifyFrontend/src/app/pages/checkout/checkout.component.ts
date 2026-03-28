import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { BookingSessionService } from '../../services/booking-session.service';
import { ReservationService } from '../../services/reservation.service';
import { BookingSession, SeatHold } from '../../models/booking.model';

@Component({
  selector: 'app-checkout',
  standalone: false,
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.sass']
})
export class CheckoutComponent implements OnInit, OnDestroy {
  checkoutForm: FormGroup;
  session: BookingSession | null = null;
  totalPrice: number = 0;
  
  isLoading: boolean = true;
  isSubmitting: boolean = false;
  errorMessage: string | null = null;
  
  private destroy$ = new Subscription();

  constructor(
    private fb: FormBuilder,
    private bookingService: BookingSessionService,
    private reservationService: ReservationService,
    private router: Router
  ) {
    this.checkoutForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern('^\\+?[0-9]{10,14}$')]]
    });
  }

  ngOnInit(): void {
    // Hardcoded session for demo/development
    this.loadSession('bs_001');
  }
  
  loadSession(sessionId: string): void {
    this.isLoading = true;
    this.errorMessage = null;
    
    this.destroy$.add(
      this.bookingService.getActiveSession(sessionId).subscribe({
        next: (session) => {
          this.session = session;
          this.calculateTotal(session.holds);
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Failed to load booking session. Please try again.';
          this.isLoading = false;
        }
      })
    );
  }
  
  calculateTotal(holds: SeatHold[]): void {
    this.totalPrice = holds.reduce((acc, hold) => acc + hold.basePrice, 0);
  }

  onSubmit(): void {
    if (this.checkoutForm.invalid || !this.session) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const request = {
      bookingSessionId: this.session.id,
      customerName: this.checkoutForm.value.fullName,
      customerEmail: this.checkoutForm.value.email,
      customerPhone: this.checkoutForm.value.phone
    };

    this.destroy$.add(
      this.reservationService.createReservation(request).subscribe({
        next: (res) => {
          this.isSubmitting = false;
          // In a real app we'd redirect to a success page with the reservation ID
          // this.router.navigate(['/booking-success', res.id]);
          alert(`Reservation successful! ID: ${res.id}`);
        },
        error: (err) => {
          this.errorMessage = 'Failed to create reservation. Please check your details and try again.';
          this.isSubmitting = false;
        }
      })
    );
  }
  
  // Helpers for template
  get f() {
    return this.checkoutForm.controls;
  }
  
  ngOnDestroy(): void {
    this.destroy$.unsubscribe();
  }
}
