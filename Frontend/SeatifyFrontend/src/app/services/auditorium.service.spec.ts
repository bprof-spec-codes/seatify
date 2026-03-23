import { TestBed } from '@angular/core/testing';

import { AuditoriumService } from './auditorium.service';

describe('AuditoriumService', () => {
  let service: AuditoriumService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuditoriumService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
