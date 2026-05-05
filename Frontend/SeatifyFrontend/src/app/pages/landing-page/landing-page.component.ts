import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { SeatMapDisplayComponent } from '../../helpers/seat-map-display/seat-map-display.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [SeatMapDisplayComponent, CommonModule],
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.sass'
})
export class LandingPageComponent {

  constructor(
    public authService: AuthService,
    private router: Router) { }

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
