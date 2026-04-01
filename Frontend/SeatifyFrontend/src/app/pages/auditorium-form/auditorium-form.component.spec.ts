import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuditoriumFormComponent } from './auditorium-form.component';

describe('AuditoriumFormComponent', () => {
  let component: AuditoriumFormComponent;
  let fixture: ComponentFixture<AuditoriumFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AuditoriumFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AuditoriumFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
