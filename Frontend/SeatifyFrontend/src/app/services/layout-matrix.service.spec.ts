import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { take } from 'rxjs';

import { LayoutMatrixService } from './layout-matrix.service';
import { ConfigService } from './config.service';
import { CreateLayoutMatrixDto, LayoutMatrix } from '../models/layout-matrix';

describe('LayoutMatrixService', () => {
  let service: LayoutMatrixService;
  let httpMock: HttpTestingController;

  const configServiceMock = {
    cfg: {
      baseApiUrl: 'http://localhost:5141'
    }
  };

  const apiUrl = `${configServiceMock.cfg.baseApiUrl}/api`;

  const mockRawMatrix = {
    id: 'matrix-1',
    auditoriumId: 'aud-1',
    name: 'Main Floor',
    rows: 10,
    columns: 20,
    createdAtUtc: '2023-10-01T12:00:00Z',
    updatedAtUtc: '2023-10-01T12:00:00Z'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        LayoutMatrixService,
        {
          provide: ConfigService,
          useValue: configServiceMock
        }
      ]
    });

    service = TestBed.inject(LayoutMatrixService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('HTTP Operations & State Management', () => {
    it('should fetch matrices, map date strings to Date objects, and update state', () => {
      const auditoriumId = 'aud-1';

      service.getLayoutMatrixByAuditoriumId(auditoriumId).subscribe((matrices) => {
        expect(matrices.length).toBe(1);
        expect(matrices[0].id).toBe('matrix-1');
        expect(matrices[0].createdAtUtc instanceof Date).toBeTrue();
      });

      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/layout-matrices`);
      expect(req.request.method).toBe('GET');

      req.flush([mockRawMatrix]);

      service.LayoutMatrix$.pipe(take(1)).subscribe(state => {
        expect(state.length).toBe(1);
        expect(state[0].id).toBe('matrix-1');
      });
    });

    it('should create a new matrix, map dates, and append to state', () => {
      const auditoriumId = 'aud-1';
      const dto: CreateLayoutMatrixDto = {
        name: 'Balcony',
        rows: 5,
        columns: 10
      };

      const newRawMatrix = {
        id: 'matrix-2',
        auditoriumId: 'aud-1',
        name: 'Balcony',
        rows: 5,
        columns: 10,
        createdAtUtc: '2023-10-02T12:00:00Z',
        updatedAtUtc: '2023-10-02T12:00:00Z'
      };

      service.setMatrices([mockRawMatrix as any]);

      service.createLayoutMatrix(dto, auditoriumId).subscribe((matrix) => {
        expect(matrix.id).toBe('matrix-2');
        expect(matrix.createdAtUtc instanceof Date).toBeTrue();
      });

      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/layout-matrices`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(dto);

      req.flush(newRawMatrix);

      service.LayoutMatrix$.pipe(take(1)).subscribe(state => {
        expect(state.length).toBe(2);
        expect(state[1].id).toBe('matrix-2');
      });
    });

    it('should update an existing matrix and modify it in the state array', () => {
      const matrixId = 'matrix-1';
      const dto: CreateLayoutMatrixDto = {
        name: 'Main Floor Updated',
        rows: 12,
        columns: 20
      };

      const updatedRawMatrix = {
        ...mockRawMatrix,
        name: 'Main Floor Updated',
        rows: 12
      };

      service.setMatrices([mockRawMatrix as any]);

      service.updateLayoutMatrix(matrixId, dto).subscribe((matrix) => {
        expect(matrix.name).toBe('Main Floor Updated');
      });

      const req = httpMock.expectOne(`${apiUrl}/layout-matrices/${matrixId}`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(dto);

      req.flush(updatedRawMatrix);

      service.LayoutMatrix$.pipe(take(1)).subscribe(state => {
        expect(state.length).toBe(1);
        expect(state[0].name).toBe('Main Floor Updated');
        expect(state[0].rows).toBe(12);
      });
    });

    it('should delete a matrix and remove it from the state array', () => {
      const matrixId = 'matrix-1';

      service.setMatrices([mockRawMatrix as any]);

      service.deleteLayoutMatrix(matrixId).subscribe();

      const req = httpMock.expectOne(`${apiUrl}/layout-matrices/${matrixId}`);
      expect(req.request.method).toBe('DELETE');

      req.flush(null);

      service.LayoutMatrix$.pipe(take(1)).subscribe(state => {
        expect(state.length).toBe(0);
      });
    });
  });

  describe('Local State Methods', () => {
    it('should manually set matrices', () => {
      const mockMatrices: LayoutMatrix[] = [
        {
          id: '1',
          auditoriumId: 'a1',
          name: 'Test',
          rows: 1,
          columns: 1,
          createdAtUtc: new Date(),
          updatedAtUtc: new Date()
        }
      ];

      service.setMatrices(mockMatrices);

      service.LayoutMatrix$.pipe(take(1)).subscribe(state => {
        expect(state).toEqual(mockMatrices);
        expect(state.length).toBe(1);
      });
    });

    it('should clear matrices state', () => {
      service.setMatrices([{ id: '1' } as LayoutMatrix]);

      service.clearMatrices();

      service.LayoutMatrix$.pipe(take(1)).subscribe(state => {
        expect(state).toEqual([]);
        expect(state.length).toBe(0);
      });
    });
  });

  describe('Error Handling', () => {
    it('should catch HTTP errors and throw a user-friendly error', (done) => {
      const auditoriumId = 'aud-1';

      service.getLayoutMatrixByAuditoriumId(auditoriumId).subscribe({
        next: () => done.fail('Expected an error, but got a successful response'),
        error: (error) => {
          expect(error.message).toBe('Something went wrong; please try again later.');
          done();
        }
      });

      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/layout-matrices`);

      req.flush('Server Error', {
        status: 500,
        statusText: 'Internal Server Error'
      });
    });
  });
});