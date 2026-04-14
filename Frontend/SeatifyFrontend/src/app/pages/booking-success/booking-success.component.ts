import { Component, OnInit } from '@angular/core';
import { BookingSessionService } from '../../services/booking-session.service';

@Component({
  selector: 'app-booking-success',
  standalone: false,
  templateUrl: './booking-success.component.html',
  styleUrl: './booking-success.component.sass'
})
export class BookingSuccessComponent {
  qrImageUrl: string | null = null;

  constructor (private bookingService: BookingSessionService) {}

  generateQrCode() {
    this.bookingService.createBookingSession().subscribe({
      next: (response) => {
        const blob = response;
        this.qrImageUrl = URL.createObjectURL(blob);
      },
      error: (err) => {
        console.error(err.message)
      }
    });
  }
}
