import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ReservationService, BookingCheckoutRequest, BookingCheckoutResponse, ReservationView } from './reservation.service'; // Adjust the import path if necessary
import { environment } from '../../environments/environment';

describe('ReservationService', () => {
  let service: ReservationService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.baseApiUrl}/api`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ReservationService]
    });
    service = TestBed.inject(ReservationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Ensure that there are no outstanding HTTP requests after each test
    httpMock.verify();
  });

  it('should be created', () => {
    // Assert
    expect(service).toBeTruthy();
  });

  describe('checkoutReservation', () => {
    it('should send a POST request to checkout and return the response', () => {
      // Arrange
      const mockRequest: BookingCheckoutRequest = {
        eventOccurrenceId: 'occ-123',
        seatIds: ['seat-1', 'seat-2'],
        customerName: 'John Doe',
        customerEmail: 'john.doe@example.com',
        customerPhone: '+1234567890',
        bookingSessionId: 'session-456'
      };

      const mockResponse: BookingCheckoutResponse = {
        bookingId: 'booking-789',
        eventId: 'occ-123',
        seats: ['seat-1', 'seat-2'],
        totalPrice: 150.00,
        qrCodeBase64: 'base64encodedQRstring'
      };

      // Act
      service.checkoutReservation(mockRequest).subscribe((response) => {
        // Assert
        expect(response).toBeTruthy();
        expect(response.bookingId).toBe('booking-789');
        expect(response.totalPrice).toBe(150.00);
        expect(response).toEqual(mockResponse);
      });

      // Assert HTTP request details
      const req = httpMock.expectOne(`${apiUrl}/bookings/checkout`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockRequest); // Verify payload

      // Flush the mocked response
      req.flush(mockResponse);
    });
  });

  describe('getReservationsForOccurrence', () => {
    it('should send a GET request and return an array of reservations', () => {
      // Arrange
      const occurrenceId = 'occ-999';
      const mockReservations: ReservationView[] = [
        {
          id: 'res-1',
          customerName: 'Alice Smith',
          customerEmail: 'alice@example.com',
          status: 'Confirmed',
          createdAtUtc: '2023-10-01T12:00:00Z',
          reservedSeats: [
            { seatId: 'seat-10', finalPrice: 75.00 },
            { seatId: 'seat-11', finalPrice: 75.00 }
          ]
        }
      ];

      // Act
      service.getReservationsForOccurrence(occurrenceId).subscribe((reservations) => {
        // Assert
        expect(reservations).toBeTruthy();
        expect(reservations.length).toBe(1);
        expect(reservations[0].customerName).toBe('Alice Smith');
        expect(reservations[0].reservedSeats.length).toBe(2);
      });

      // Assert HTTP request details
      const req = httpMock.expectOne(`${apiUrl}/by-event-occurrences/${occurrenceId}/reservations`);
      expect(req.request.method).toBe('GET');

      // Flush the mocked response
      req.flush(mockReservations);
    });
  });
});