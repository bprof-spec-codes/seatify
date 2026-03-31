import { TestBed } from '@angular/core/testing';

import { SeatService } from './seat.service';

describe('SeatServiceService', () => {
  let service: SeatService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SeatService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
