import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BookingSessionService } from './booking-session.service';
import { ConfigService } from './config.service';
import { BookingSession } from '../models/booking.model';

describe('BookingSessionService', () => {
  let service: BookingSessionService;
  let httpMock: HttpTestingController;

  const configServiceMock = {
    cfg: {
      baseApiUrl: 'http://localhost:5141'
    },
    apiBaseUrl: 'http://localhost:5141'
  };

  const baseApi = configServiceMock.cfg.baseApiUrl;
  const apiUrl = `${baseApi}/api/public/booking-sessions`;

  const makeSession = (overrides: Partial<BookingSession> = {}): BookingSession => ({
    id: overrides.id ?? 's1',
    eventOccurrenceId: overrides.eventOccurrenceId ?? 'e1',
    phase: overrides.phase ?? 'Selection',
    status: overrides.status ?? 'Active',
    expiresAtUtc: overrides.expiresAtUtc ?? new Date(Date.now() + 3600_000).toISOString(),
    holds: overrides.holds ?? [],
    ...overrides
  });

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        BookingSessionService,
        { provide: ConfigService, useValue: configServiceMock }
      ]
    });

    service = TestBed.inject(BookingSessionService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should GET active session by id', () => {
    const sessionId = 's123';
    const mockSession: BookingSession = makeSession({ id: sessionId });

    service.getActiveSession(sessionId).subscribe(resp => {
      expect(resp).toEqual(mockSession);
    });

    const req = httpMock.expectOne(`${apiUrl}/${sessionId}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockSession);
  });

  it('should POST to checkout and return void', () => {
    const sessionId = 's-check';

    service.checkout(sessionId).subscribe(resp => {
      expect(resp).toBeNull();
    });

    const req = httpMock.expectOne(`${apiUrl}/${sessionId}/checkout`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});
    req.flush(null);
  });

  it('should create booking session (POST) with eventOccurrenceId', () => {
    const eventOccurrenceId = 'e1';
    const mockSession: BookingSession = makeSession({ id: 'new', eventOccurrenceId });

    service.createBookingSession(eventOccurrenceId).subscribe(resp => {
      expect(resp).toEqual(mockSession);
    });

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ eventOccurrenceId });
    req.flush(mockSession);
  });

  it('should hold a seat (POST) and return BookingSession with holds', () => {
    const sessionId = 's1';
    const seatId = 'seatA';
    const mockSession: BookingSession = makeSession({ id: sessionId, holds: [{ seatId, heldAtUtc: new Date().toISOString() } as any] });

    service.holdSeat(sessionId, seatId).subscribe(resp => {
      expect(resp).toEqual(mockSession);
    });

    const req = httpMock.expectOne(`${apiUrl}/${sessionId}/holds`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ seatId });
    req.flush(mockSession);
  });

  it('should release a seat (DELETE) and return BookingSession with holds updated', () => {
    const sessionId = 's1';
    const seatId = 'seatA';
    const mockSessionBefore: BookingSession = makeSession({
      id: sessionId,
      holds: [{ seatId, heldAtUtc: new Date().toISOString() } as any]
    });
    const mockSessionAfter: BookingSession = makeSession({ id: sessionId, holds: [] });

    service.releaseSeat(sessionId, seatId).subscribe(resp => {
      expect(resp).toEqual(mockSessionAfter);
    });

    const req = httpMock.expectOne(`${apiUrl}/${sessionId}/holds/${seatId}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(mockSessionAfter);
  });

  it('private api(path) should use ConfigService.cfg.baseApiUrl', () => {
    expect((TestBed.inject(ConfigService) as any).cfg.baseApiUrl).toBe(baseApi);
  });
});
