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
    window.print();
  }

  finish(): void {
    this.stateService.clearState();
    this.router.navigate(['/']);
  }
}
