import { Component, OnInit } from '@angular/core';
import { Auditorium } from '../../models/auditorium';
import { ActivatedRoute, Router } from '@angular/router';
import { AuditoriumService } from '../../services/auditorium.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-auditorium-dashboard',
  standalone: false,
  templateUrl: './auditorium-dashboard.component.html',
  styleUrl: './auditorium-dashboard.component.sass'
})
export class AuditoriumDashboardComponent implements OnInit {
  venueId!: string;
  auditoriums!: Observable<Auditorium[]>;

  constructor(
    private router: Router, 
    private activatedRoute: ActivatedRoute, 
    private auditoriumService: AuditoriumService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.venueId = params['venueId'];
      this.auditoriums = this.auditoriumService.getAuditoriumsByVenueId(this.venueId);
    });
  }

  back(): void {
    this.router.navigate(['/venues']);
  }
}
