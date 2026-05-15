import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CheckInService, CheckInResult, TicketStatus } from './checkin.service';
import { environment } from '../../environments/environment';

describe('CheckInService', () => {
  let service: CheckInService;
  let httpMock: HttpTestingController;
  const baseApi = `${environment.baseApiUrl}/api/checkin`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [CheckInService]
    });

    service = TestBed.inject(CheckInService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('validateTicket should POST to /validate with payload and return CheckInResult', (done) => {
    const payload = 'encodedTicket';
    const mockResponse: CheckInResult = {
      ticketId: 't123',
      status: TicketStatus.Valid,
      statusMessage: 'OK',
      reservation: { customerName: 'Alice', customerEmail: 'alice@example.com' },
      seat: { section: 'A', row: 1, number: 10 },
      event: { title: 'Concert', startTime: '2026-06-01T20:00:00Z' }
    };

    service.validateTicket(payload).subscribe((res: any) => {
      expect(res).toEqual(mockResponse);
      done();
    });

    const req = httpMock.expectOne(`${baseApi}/validate`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ payload });
    req.flush(mockResponse);
  });

  it('confirmCheckIn should POST to /confirm with ticketId and return CheckInResult', (done) => {
    const ticketId = 't123';
    const mockResponse: CheckInResult = {
      ticketId,
      status: TicketStatus.AlreadyUsed,
      statusMessage: 'Already used'
    };

    service.confirmCheckIn(ticketId).subscribe((res: any) => {
      expect(res).toEqual(mockResponse);
      done();
    });

    const req = httpMock.expectOne(`${baseApi}/confirm`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ ticketId });
    req.flush(mockResponse);
  });

  it('should propagate HTTP error from validateTicket', (done) => {
    const payload = 'bad';
    const mockError = { status: 400, statusText: 'Bad Request' };

    service.validateTicket(payload).subscribe({
      next: () => fail('expected an error'),
      error: (err: any) => {
        expect(err.status).toBe(400);
        done();
      }
    });

    const req = httpMock.expectOne(`${baseApi}/validate`);
    req.flush({ message: 'invalid' }, mockError);
  });

  it('should propagate HTTP error from confirmCheckIn', (done) => {
    const ticketId = 'bad-id';
    const mockError = { status: 500, statusText: 'Server Error' };

    service.confirmCheckIn(ticketId).subscribe({
      next: () => fail('expected an error'),
      error: (err: any) => {
        expect(err.status).toBe(500);
        done();
      }
    });

    const req = httpMock.expectOne(`${baseApi}/confirm`);
    req.flush({ message: 'error' }, mockError);
  });
});
