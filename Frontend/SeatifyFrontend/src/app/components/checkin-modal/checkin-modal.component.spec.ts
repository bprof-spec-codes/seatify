import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckinModalComponent } from './checkin-modal.component';

describe('CheckinModalComponent', () => {
  let component: CheckinModalComponent;
  let fixture: ComponentFixture<CheckinModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CheckinModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CheckinModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
