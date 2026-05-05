import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SeatOverrideService } from './seat-override.service';
import { environment } from '../../environments/environment';
import {
  BulkSeatOverrideDto,
  BulkSeatOverrideResponseDto,
  EffectiveSeatMap,
  EffectiveSeat
} from '../models/seat-override';

describe('SeatOverrideService', () => {
  let service: SeatOverrideService;
  let httpTestingController: HttpTestingController;

  const apiUrl = `${environment.baseApiUrl}/api`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SeatOverrideService]
    });

    service = TestBed.inject(SeatOverrideService);
    httpTestingController = TestBed.inject(HttpTestingController);

    // Suppress console.error in tests to keep the console clean
    spyOn(console, 'error');
  });

  afterEach(() => {
    // Assert that there are no outstanding requests
    httpTestingController.verify();
  });

  it('should be created', () => {
    // Assert
    expect(service).toBeTruthy();
  });

  // --------------------------------------------------------------------------
  // getEffectiveSeatMapForEvent Tests
  // --------------------------------------------------------------------------

  it('getEffectiveSeatMapForEvent should send GET request and return map', () => {
    // Arrange
    const eventId = 'event-1';
    const matrixId = 'matrix-1';
    const mockSeat: EffectiveSeat = {
      seatId: 'seat-1',
      matrixId: 'matrix-1',
      row: 1,
      column: 1,
      seatLabel: 'A1',
      sectorId: 'sec-1',
      seatType: 'Seat',
      priceOverride: 1500,
      finalPrice: 1500,
      sectorSource: 'event',
      seatTypeSource: 'auditorium',
      priceSource: 'event'
    };
    const mockResponse: EffectiveSeatMap = {
      matrixId: 'matrix-1',
      matrixName: 'Main Layout',
      rows: 10,
      columns: 10,
      context: 'event',
      eventId: 'event-1',
      occurrenceId: null,
      seats: [mockSeat]
    };

    // Act
    service.getEffectiveSeatMapForEvent(eventId, matrixId).subscribe((res) => {
      // Assert
      expect(res).toBeTruthy();
      expect(res.matrixId).toBe('matrix-1');
      expect(res.seats.length).toBe(1);
      expect(res.seats[0].finalPrice).toBe(1500);
    });

    const req = httpTestingController.expectOne(`${apiUrl}/events/${eventId}/seat-map/${matrixId}`);
    expect(req.request.method).toEqual('GET');
    req.flush(mockResponse);
  });

  it('getEffectiveSeatMapForEvent should handle HTTP errors gracefully', () => {
    // Arrange
    const eventId = 'event-1';
    const matrixId = 'matrix-1';

    // Act
    service.getEffectiveSeatMapForEvent(eventId, matrixId).subscribe({
      next: () => fail('Should have failed with a generic error message'),
      error: (error) => {
        // Assert
        expect(error.message).toBe('Something went wrong; please try again later.');
        expect(console.error).toHaveBeenCalled();
      }
    });

    const req = httpTestingController.expectOne(`${apiUrl}/events/${eventId}/seat-map/${matrixId}`);
    req.flush('Server Error', { status: 500, statusText: 'Internal Server Error' });
  });

  // --------------------------------------------------------------------------
  // bulkUpsertEventOverride Tests
  // --------------------------------------------------------------------------

  it('bulkUpsertEventOverride should send PATCH request and return response', () => {
    // Arrange
    const eventId = 'event-1';
    const dto: BulkSeatOverrideDto = {
      seatIds: ['seat-1', 'seat-2'],
      priceOverride: 5000
    };
    const mockResponse: BulkSeatOverrideResponseDto = {
      upsertedCount: 2,
      affectedSeatIds: ['seat-1', 'seat-2']
    };

    // Act
    service.bulkUpsertEventOverride(eventId, dto).subscribe((res) => {
      // Assert
      expect(res).toBeTruthy();
      expect(res.upsertedCount).toBe(2);
      expect(res.affectedSeatIds).toContain('seat-1');
    });

    const req = httpTestingController.expectOne(`${apiUrl}/events/${eventId}/seats/bulk`);
    expect(req.request.method).toEqual('PATCH');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResponse);
  });

  it('bulkUpsertEventOverride should handle HTTP errors gracefully', () => {
    // Arrange
    const eventId = 'event-error';
    const dto: BulkSeatOverrideDto = { seatIds: [] };

    // Act
    service.bulkUpsertEventOverride(eventId, dto).subscribe({
      next: () => fail('Should have failed'),
      error: (error) => {
        // Assert
        expect(error.message).toBe('Something went wrong; please try again later.');
      }
    });

    const req = httpTestingController.expectOne(`${apiUrl}/events/${eventId}/seats/bulk`);
    req.flush('Bad Request', { status: 400, statusText: 'Bad Request' });
  });

  // --------------------------------------------------------------------------
  // getEffectiveSeatMapForOccurrence Tests
  // --------------------------------------------------------------------------

  it('getEffectiveSeatMapForOccurrence should send GET request and return map', () => {
    // Arrange
    const occurrenceId = 'occ-1';
    const matrixId = 'mat-1';
    const mockResponse: EffectiveSeatMap = {
      matrixId: 'mat-1',
      matrixName: 'Occ Layout',
      rows: 5,
      columns: 5,
      context: 'occurrence',
      eventId: 'evt-1',
      occurrenceId: 'occ-1',
      seats: []
    };

    // Act
    service.getEffectiveSeatMapForOccurrence(occurrenceId, matrixId).subscribe((res) => {
      // Assert
      expect(res).toBeTruthy();
      expect(res.context).toBe('occurrence');
      expect(res.occurrenceId).toBe('occ-1');
    });

    const req = httpTestingController.expectOne(`${apiUrl}/event-occurrences/${occurrenceId}/seat-map/${matrixId}`);
    expect(req.request.method).toEqual('GET');
    req.flush(mockResponse);
  });

  it('getEffectiveSeatMapForOccurrence should handle HTTP errors gracefully', () => {
    // Arrange
    const occurrenceId = 'occ-err';
    const matrixId = 'mat-err';

    // Act
    service.getEffectiveSeatMapForOccurrence(occurrenceId, matrixId).subscribe({
      next: () => fail('Should have failed'),
      error: (error) => {
        // Assert
        expect(error.message).toBe('Something went wrong; please try again later.');
      }
    });

    const req = httpTestingController.expectOne(`${apiUrl}/event-occurrences/${occurrenceId}/seat-map/${matrixId}`);
    req.flush('Not Found', { status: 404, statusText: 'Not Found' });
  });

  // --------------------------------------------------------------------------
  // bulkUpsertOccurrenceOverride Tests
  // --------------------------------------------------------------------------

  it('bulkUpsertOccurrenceOverride should send PATCH request and return response', () => {
    // Arrange
    const occurrenceId = 'occ-1';
    const dto: BulkSeatOverrideDto = {
      seatIds: ['s1'],
      clearPriceOverride: true
    };
    const mockResponse: BulkSeatOverrideResponseDto = {
      upsertedCount: 1,
      affectedSeatIds: ['s1']
    };

    // Act
    service.bulkUpsertOccurrenceOverride(occurrenceId, dto).subscribe((res) => {
      // Assert
      expect(res).toBeTruthy();
      expect(res.upsertedCount).toBe(1);
    });

    const req = httpTestingController.expectOne(`${apiUrl}/event-occurrences/${occurrenceId}/seats/bulk`);
    expect(req.request.method).toEqual('PATCH');
    expect(req.request.body).toEqual(dto);
    req.flush(mockResponse);
  });

  it('bulkUpsertOccurrenceOverride should handle HTTP errors gracefully', () => {
    // Arrange
    const occurrenceId = 'occ-error';
    const dto: BulkSeatOverrideDto = { seatIds: [] };

    // Act
    service.bulkUpsertOccurrenceOverride(occurrenceId, dto).subscribe({
      next: () => fail('Should have failed'),
      error: (error) => {
        // Assert
        expect(error.message).toBe('Something went wrong; please try again later.');
      }
    });

    const req = httpTestingController.expectOne(`${apiUrl}/event-occurrences/${occurrenceId}/seats/bulk`);
    req.flush('Error', { status: 500, statusText: 'Server Error' });
  });
});