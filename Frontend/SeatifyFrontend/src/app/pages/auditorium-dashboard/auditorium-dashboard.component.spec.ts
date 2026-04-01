import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuditoriumDashboardComponent } from './auditorium-dashboard.component';

describe('AuditoriumDashboardComponent', () => {
  let component: AuditoriumDashboardComponent;
  let fixture: ComponentFixture<AuditoriumDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AuditoriumDashboardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuditoriumDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
