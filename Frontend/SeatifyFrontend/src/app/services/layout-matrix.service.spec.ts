import { TestBed } from '@angular/core/testing';

import { LayoutMatrixService } from './layout-matrix.service';

describe('LayoutMatrixService', () => {
  let service: LayoutMatrixService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LayoutMatrixService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
