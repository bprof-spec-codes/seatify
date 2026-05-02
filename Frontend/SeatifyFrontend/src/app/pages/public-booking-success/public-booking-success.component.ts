import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PublicBookingStateService } from '../../services/public-booking-state.service';

@Component({
  selector: 'app-public-booking-success',
  standalone: false,
  templateUrl: './public-booking-success.component.html',
  styleUrls: ['./public-booking-success.component.sass']
})
export class PublicBookingSuccessComponent implements OnInit {
  result: any = null;
  occ: any = null;
  now = new Date();

  constructor(
    private stateService: PublicBookingStateService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.result = (this.stateService as any).lastCheckoutResult;
    this.occ = this.stateService.getEventOccurrence();
    
    if (!this.result) {
      this.router.navigate(['/']);
    }
  }

  printTickets(): void {
    if (this.result && this.result.pdfBase64) {
      const byteCharacters = atob(this.result.pdfBase64);
      const byteNumbers = new Array(byteCharacters.length);
      for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
      }
      const byteArray = new Uint8Array(byteNumbers);
      const blob = new Blob([byteArray], { type: 'application/pdf' });
      
      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = `Seatify_Tickets_${this.result.bookingId.slice(0, 8)}.pdf`;
      link.click();
    } else {
      window.print();
    }
  }

  finish(): void {
    this.stateService.clearState();
    this.router.navigate(['/']);
  }
}
