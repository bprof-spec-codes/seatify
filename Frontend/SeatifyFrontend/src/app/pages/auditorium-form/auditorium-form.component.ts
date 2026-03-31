import { Component, OnDestroy, OnInit } from '@angular/core';
import { Auditorium } from '../../models/auditorium';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, Subject, of, takeUntil } from 'rxjs';
import { AuditoriumService } from '../../services/auditorium.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-auditorium-form',
  standalone: false,
  templateUrl: './auditorium-form.component.html',
  styleUrl: './auditorium-form.component.sass'
})
export class AuditoriumFormComponent implements OnInit, OnDestroy {
  auditoriumForm!: FormGroup;
  auditorium!: Auditorium;
  venueId!: string;
  editMode!: boolean;
  editMode$!: Observable<boolean>;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private auditoriumService: AuditoriumService
  ) {}

  ngOnInit(): void {
    this.venueId = this.route.snapshot.paramMap.get('venueId') || '';

    this.auditoriumForm = this.fb.group({
      auditoriumName: ['', [Validators.required, Validators.maxLength(100)]],
      auditoriumDescription: ['', [Validators.maxLength(500)]]
    });

    this.auditoriumService.editMode$.pipe(takeUntil(this.unsubscribe$)).subscribe(editMode => {
      this.editMode = editMode;
      this.editMode$ = of(editMode);
      if (this.editMode) {
        this.fetchAuditorium();
      }
    });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onSubmit() {
    if (this.auditoriumForm.valid) {
      const auditoriumData: Auditorium = {
        id: this.editMode && this.auditorium ? this.auditorium.id : '',
        venueId: this.venueId,
        name: this.auditoriumForm.value.auditoriumName,
        description: this.auditoriumForm.value.auditoriumDescription,
        createdAtUtc: this.editMode && this.auditorium ? this.auditorium.createdAtUtc : new Date(),
        updatedAtUtc: new Date()
      };

      if (this.editMode) {
        this.auditoriumService.updateAuditorium(auditoriumData).pipe(takeUntil(this.unsubscribe$)).subscribe(() => {
          this.router.navigate([`dashboard/auditoriums/${this.venueId}`]);
        });
      } else {
        this.auditoriumService.createAuditorium(this.venueId, auditoriumData).pipe(takeUntil(this.unsubscribe$)).subscribe(() => {
          this.router.navigate([`dashboard/auditoriums/${this.venueId}`]);
        });
      }
    }
  }

  back(): void {
    this.router.navigate([`dashboard/auditoriums/${this.venueId}`]);
  }

  private fetchAuditorium(): void {
    this.auditoriumService.getEditAuditorium().pipe(takeUntil(this.unsubscribe$)).subscribe(auditorium => {
      this.auditorium = auditorium;
      this.auditoriumForm.patchValue({
        auditoriumName: this.auditorium.name,
        auditoriumDescription: this.auditorium.description
      });
    });
  }
}

