import { Component, OnInit } from '@angular/core';
import { Auditorium } from '../../models/auditorium';
import { ActivatedRoute, Router } from '@angular/router';
import { AuditoriumService } from '../../services/auditorium.service';
import { Observable, of, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-auditorium-dashboard',
  standalone: false,
  templateUrl: './auditorium-dashboard.component.html',
  styleUrl: './auditorium-dashboard.component.sass'
})
export class AuditoriumDashboardComponent implements OnInit {
  venueId!: string;
  auditoriums$!: Observable<Auditorium[]>;
  auditoriums!: Auditorium[];
  showModal: boolean = false;
  selectedAuditorium!: Auditorium;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private router: Router, 
    private activatedRoute: ActivatedRoute, 
    private auditoriumService: AuditoriumService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.venueId = params['venueId'];
      //this.auditoriums$ = this.auditoriumService.getAuditoriumsByVenueId(this.venueId);
    });

    this.auditoriumService.getAuditoriumsByVenueId(this.venueId).pipe(takeUntil(this.unsubscribe$)).subscribe(auditoriums => {
      this.auditoriums = auditoriums;
      this.auditoriums$ = of(this.auditoriums);
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  back(): void {
    this.router.navigate(['/venues']);
  }

  viewAuditorium(auditorium: Auditorium): void {
    console.log('View auditorium! ', auditorium);
  }

  createAuditorium(): void {
    console.log('Create auditorium!');
  }

  editAuditorium(auditorium: Auditorium): void {
    console.log('Edit auditorium! ', auditorium);
  }

  confirmDelete(auditorium: Auditorium): void {
    this.selectedAuditorium = auditorium;
    this.showModal = true;
  }

  deleteAuditorium(auditorium: Auditorium): void {
    this.auditoriumService.deleteAuditoriumById(auditorium.id).subscribe({
      next: () => {
        console.log('Auditorium successfully deleted!');
        this.auditoriums = this.auditoriums.filter(a => a.id !== auditorium.id);
        this.auditoriums$ = of(this.auditoriums);
        //this.auditoriums$ = this.auditoriumService.getAuditoriumsByVenueId(this.venueId);
      },
      error: err => console.error('Error: ', err.message)
    });

    this.cancelDelete();
  }
  
  cancelDelete(): void {
    this.showModal = false;
  }
}
