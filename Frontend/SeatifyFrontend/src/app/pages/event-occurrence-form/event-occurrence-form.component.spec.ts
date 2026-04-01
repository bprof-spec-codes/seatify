import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventOccurrenceFormComponent } from './event-occurrence-form.component';

describe('EventOccurrenceFormComponent', () => {
  let component: EventOccurrenceFormComponent;
  let fixture: ComponentFixture<EventOccurrenceFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EventOccurrenceFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventOccurrenceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
