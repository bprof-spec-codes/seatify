import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeatMap3dComponent } from './seat-map3d.component';

describe('SeatMap3dComponent', () => {
  let component: SeatMap3dComponent;
  let fixture: ComponentFixture<SeatMap3dComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SeatMap3dComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SeatMap3dComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
