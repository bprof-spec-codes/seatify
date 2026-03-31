import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmMessageComponent } from './confirm-message.component';

describe('ConfirmMessageComponent', () => {
  let component: ConfirmMessageComponent;
  let fixture: ComponentFixture<ConfirmMessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ConfirmMessageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfirmMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
