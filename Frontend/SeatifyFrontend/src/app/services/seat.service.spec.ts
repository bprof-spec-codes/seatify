import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SeatService } from './seat.service';
import { environment } from '../../environments/environment';
import { Seat, SeatType, UpdateSeatDto, BulkSeatUpdateDto } from '../models/seat';
import { SeatMap } from '../models/seat-map';

describe('SeatService', () => {
  let service: SeatService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.baseApiUrl}/api`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SeatService]
    });
    service = TestBed.inject(SeatService);
    httpMock = TestBed.inject(HttpTestingController);

    // Suppress console.error in tests to keep the output clean
    spyOn(console, 'error');
  });

  afterEach(() => {
    // Assert that there are no outstanding requests
    httpMock.verify();
  });

  it('should be created', () => {
    // Assert
    expect(service).toBeTruthy();
  });

  it('getSeatMapByMatrixId should fetch, map dates, and update BehaviorSubject', () => {
    // Arrange
    const matrixId = 'mat-123';
    // Simulating API response where dates come as ISO strings
    const mockApiResponse = {
      id: 'map-1',
      rows: 5,
      columns: 5,
      seats: [
        {
          id: 'seat-1',
          matrixId: matrixId,
          row: 1,
          column: 1,
          seatType: SeatType.Seat,
          createdAtUtc: '2023-10-01T12:00:00Z' as any, 
          updatedAtUtc: '2023-10-01T12:00:00Z' as any
        }
      ]
    } as SeatMap;

    // Act
    service.getSeatMapByMatrixId(matrixId).subscribe(seatMap => {
      // Assert
      expect(seatMap).toBeTruthy();
      expect(seatMap.rows).toBe(5);
      expect(seatMap.columns).toBe(5);
      expect(seatMap.seats.length).toBe(1);
      // Verify that string dates were converted to Date objects
      expect(seatMap.seats[0].createdAtUtc instanceof Date).toBeTrue();
      expect(seatMap.seats[0].updatedAtUtc instanceof Date).toBeTrue();
    });

    const req = httpMock.expectOne(`${apiUrl}/layout-matrices/${matrixId}/seat-map`);
    expect(req.request.method).toBe('GET');
    req.flush(mockApiResponse);

    // Verify BehaviorSubject is updated
    service.seatMap$.subscribe(stateMap => {
      expect(stateMap).toBeTruthy();
      expect(stateMap?.seats[0].id).toBe('seat-1');
    });
  });

  it('getSeatMapByMatrixId should handle http errors', () => {
    // Arrange
    const matrixId = 'invalid-mat';

    // Act
    service.getSeatMapByMatrixId(matrixId).subscribe({
      next: () => fail('Should have failed with the generic error message'),
      error: (err) => {
        // Assert
        expect(err.message).toBe('Something went wrong; please try again later.');
        expect(console.error).toHaveBeenCalled();
      }
    });

    const req = httpMock.expectOne(`${apiUrl}/layout-matrices/${matrixId}/seat-map`);
    expect(req.request.method).toBe('GET');
    req.flush('Error', { status: 500, statusText: 'Server Error' });
  });

  it('updateSeat should update seat, map dates, and conditionally update seatSource', () => {
    // Arrange
    const seatId = 'seat-1';
    const dto: UpdateSeatDto = {
      seatLabel: 'A1',
      sectorId: 'sec-1',
      priceOverride: 1500,
      seatType: SeatType.AccessibleSeat
    };

    const mockApiSeatResponse = {
      id: seatId,
      matrixId: 'mat-1',
      row: 1,
      column: 1,
      seatLabel: 'A1',
      sectorId: 'sec-1',
      priceOverride: 1500,
      seatType: SeatType.AccessibleSeat,
      createdAtUtc: '2023-10-01T12:00:00Z' as any,
      updatedAtUtc: '2023-10-02T12:00:00Z' as any
    } as Seat;

    // Seed the BehaviorSubject with the current seat so we can test if it gets updated
    (service as any).seatSource.next({ id: seatId, seatLabel: 'OldLabel' } as Seat);

    // Act
    service.updateSeat(seatId, dto).subscribe(updatedSeat => {
      // Assert
      expect(updatedSeat.seatLabel).toBe('A1');
      expect(updatedSeat.createdAtUtc instanceof Date).toBeTrue();
      expect(updatedSeat.updatedAtUtc instanceof Date).toBeTrue();
    });

    const req = httpMock.expectOne(`${apiUrl}/seats/${seatId}`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(dto);
    req.flush(mockApiSeatResponse);

    // Verify seatSource was updated
    service.seat$.subscribe(seat => {
      expect(seat).toBeTruthy();
      expect(seat?.seatLabel).toBe('A1');
      expect(seat?.updatedAtUtc instanceof Date).toBeTrue();
    });
  });

  it('updateSeat should handle http errors', () => {
    // Arrange
    const seatId = 'seat-error';
    const dto: UpdateSeatDto = { seatLabel: 'A1', sectorId: null, priceOverride: null, seatType: SeatType.Seat };

    // Act
    service.updateSeat(seatId, dto).subscribe({
      next: () => fail('Should have failed'),
      error: (err) => {
        // Assert
        expect(err.message).toBe('Something went wrong; please try again later.');
        expect(console.error).toHaveBeenCalled();
      }
    });

    const req = httpMock.expectOne(`${apiUrl}/seats/${seatId}`);
    expect(req.request.method).toBe('PUT');
    req.flush('Error', { status: 400, statusText: 'Bad Request' });
  });

  it('bulkUpdateSeats should send patch request and return response', () => {
    // Arrange
    const dto: BulkSeatUpdateDto = {
      seatIds: ['seat-1', 'seat-2'],
      seatType: SeatType.Aisle
    };
    const mockResponse = { updatedCount: 2, updatedSeatIds: ['seat-1', 'seat-2'] };

    // Act
    service.bulkUpdateSeats(dto).subscribe(res => {
      // Assert
      expect(res.updatedCount).toBe(2);
    });

    const req = httpMock.expectOne(`${apiUrl}/seats/bulk`);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResponse);
  });

  it('bulkUpdateSeats should handle http errors', () => {
    // Arrange
    const dto: BulkSeatUpdateDto = { seatIds: [] };

    // Act
    service.bulkUpdateSeats(dto).subscribe({
      next: () => fail('Should have failed'),
      error: (err) => {
        // Assert
        expect(err.message).toBe('Something went wrong; please try again later.');
      }
    });

    const req = httpMock.expectOne(`${apiUrl}/seats/bulk`);
    expect(req.request.method).toBe('PATCH');
    req.flush('Error', { status: 500, statusText: 'Server Error' });
  });

  it('clearSeatMap should emit null to seatMap$', () => {
    // Arrange
    (service as any).seatMapSource.next({ id: 'map-1', rows: 10, columns: 10, seats: [] } as SeatMap);

    // Act
    service.clearSeatMap();

    // Assert
    service.seatMap$.subscribe(seatMap => {
      expect(seatMap).toBeNull();
    });
  });

  it('setSeatMap should emit provided value to seatMap$', () => {
    // Arrange
    const newSeatMap = { id: 'map-99', rows: 10, columns: 10, seats: [] } as SeatMap;

    // Act
    service.setSeatMap(newSeatMap);

    // Assert
    service.seatMap$.subscribe(seatMap => {
      expect(seatMap).toEqual(newSeatMap);
    });
  });
});