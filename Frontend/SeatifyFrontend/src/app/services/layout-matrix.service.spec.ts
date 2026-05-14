import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LayoutMatrixService } from './layout-matrix.service';
import { environment } from '../../environments/environment';
import { CreateLayoutMatrixDto, LayoutMatrix } from '../models/layout-matrix';

describe('LayoutMatrixService', () => {
  let service: LayoutMatrixService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.baseApiUrl}/api`;

  // Mock data representing the raw JSON response from the backend (dates as strings)
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
      providers: [LayoutMatrixService]
    });
    service = TestBed.inject(LayoutMatrixService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Verify that no unmatched requests are outstanding
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('HTTP Operations & State Management', () => {
    it('should fetch matrices, map date strings to Date objects, and update state', () => {
      const auditoriumId = 'aud-1';

      // Act: Subscribe to the method
      service.getLayoutMatrixByAuditoriumId(auditoriumId).subscribe((matrices) => {
        // Assert: Check response mapping
        expect(matrices.length).toBe(1);
        expect(matrices[0].id).toBe('matrix-1');
        expect(matrices[0].createdAtUtc instanceof Date).toBeTrue(); // Verify date mapping
      });

      // Assert HTTP request
      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/layout-matrices`);
      expect(req.request.method).toBe('GET');
      
      // Flush raw string date data simulating backend JSON
      req.flush([mockRawMatrix]);

      // Assert: Verify BehaviorSubject state was updated
      service.LayoutMatrix$.subscribe(state => {
        expect(state.length).toBe(1);
        expect(state[0].id).toBe('matrix-1');
      });
    });

    it('should create a new matrix, map dates, and append to state', () => {
      const auditoriumId = 'aud-1';
      const dto: CreateLayoutMatrixDto = { name: 'Balcony', rows: 5, columns: 10 };
      
      const newRawMatrix = {
        id: 'matrix-2',
        auditoriumId: 'aud-1',
        name: 'Balcony',
        rows: 5,
        columns: 10,
        createdAtUtc: '2023-10-02T12:00:00Z',
        updatedAtUtc: '2023-10-02T12:00:00Z'
      };

      // Populate initial state to test appending
      service.setMatrices([mockRawMatrix as any]);

      // Act
      service.createLayoutMatrix(dto, auditoriumId).subscribe((matrix) => {
        // Assert response
        expect(matrix.id).toBe('matrix-2');
        expect(matrix.createdAtUtc instanceof Date).toBeTrue();
      });

      // Assert HTTP request
      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/layout-matrices`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(dto);
      
      // Flush response
      req.flush(newRawMatrix);

      // Assert state now contains both matrices
      service.LayoutMatrix$.subscribe(state => {
        expect(state.length).toBe(2);
        expect(state[1].id).toBe('matrix-2');
      });
    });

    it('should update an existing matrix and modify it in the state array', () => {
      const matrixId = 'matrix-1';
      const dto: CreateLayoutMatrixDto = { name: 'Main Floor Updated', rows: 12, columns: 20 };
      
      const updatedRawMatrix = {
        ...mockRawMatrix,
        name: 'Main Floor Updated',
        rows: 12
      };

      // Set initial state
      service.setMatrices([mockRawMatrix as any]);

      // Act
      service.updateLayoutMatrix(matrixId, dto).subscribe((matrix) => {
        expect(matrix.name).toBe('Main Floor Updated');
      });

      // Assert HTTP request
      const req = httpMock.expectOne(`${apiUrl}/layout-matrices/${matrixId}`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(dto);
      
      req.flush(updatedRawMatrix);

      // Assert state was updated
      service.LayoutMatrix$.subscribe(state => {
        expect(state.length).toBe(1);
        expect(state[0].name).toBe('Main Floor Updated');
        expect(state[0].rows).toBe(12);
      });
    });

    it('should delete a matrix and remove it from the state array', () => {
      const matrixId = 'matrix-1';
      
      // Set initial state with one item
      service.setMatrices([mockRawMatrix as any]);

      // Act
      service.deleteLayoutMatrix(matrixId).subscribe();

      // Assert HTTP request
      const req = httpMock.expectOne(`${apiUrl}/layout-matrices/${matrixId}`);
      expect(req.request.method).toBe('DELETE');
      
      req.flush(null); // DELETE usually returns empty body

      // Assert state is now empty
      service.LayoutMatrix$.subscribe(state => {
        expect(state.length).toBe(0);
      });
    });
  });

  describe('Local State Methods', () => {
    it('should manually set matrices', () => {
      const mockMatrices: LayoutMatrix[] = [
        { id: '1', auditoriumId: 'a1', name: 'Test', rows: 1, columns: 1, createdAtUtc: new Date(), updatedAtUtc: new Date() }
      ];

      service.setMatrices(mockMatrices);

      service.LayoutMatrix$.subscribe(state => {
        expect(state).toEqual(mockMatrices);
        expect(state.length).toBe(1);
      });
    });

    it('should clear matrices state', () => {
      // Set initial state first
      service.setMatrices([{ id: '1' } as LayoutMatrix]);
      
      // Act
      service.clearMatrices();

      // Assert
      service.LayoutMatrix$.subscribe(state => {
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
          // Assert that the error was caught and mapped to the generic message
          expect(error.message).toBe('Something went wrong; please try again later.');
          done();
        }
      });

      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/layout-matrices`);
      
      // Simulate a 500 Internal Server Error
      req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });
    });
  });
});