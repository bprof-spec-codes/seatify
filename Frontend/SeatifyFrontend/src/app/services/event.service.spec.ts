import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { EventService } from './event.service';
import { HttpErrorResponse } from '@angular/common/http';
import { EventOccurrence } from '../models/event-occurrence';
import { environment } from '../../environments/environment';
import { SeatifyEvent } from '../models/event';
import EventRequest from '../models/event.request';
import EventResponse from '../models/event.response';

describe('EventService', () => {
  let service: EventService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.baseApiUrl}/api`;
  const eventsApi = `${apiUrl}/events`;
  const occurrencesApi = `${apiUrl}/event-occurrences`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [EventService]
    });
    service = TestBed.inject(EventService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('uploadImage should post FormData and return url', () => {
    const blob = new Blob(['a'], { type: 'text/plain' });
    const file = new File([blob], 'file.txt');
    const mockResp = { url: 'http://cdn/image.jpg' };

    service.uploadImage(file).subscribe(res => {
      expect(res).toEqual(mockResp);
    });

    const req = httpMock.expectOne(`${apiUrl}/upload`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBeTruthy();
    req.flush(mockResp);
  });

  it('createEvent should post and return EventResponse', () => {
    const reqBody: EventRequest = {
      organizerId: 'org1',
      slug: 'slug',
      name: 'Name',
      description: 'Desc',
      status: 'Draft'
    };
    const resp: EventResponse = {
      id: 'e1',
      organizerId: 'org1',
      slug: 'slug',
      name: 'Name',
      description: 'Desc',
      status: 'Draft',
      createdAtUtc: new Date().toISOString(),
      updatedAtUtc: new Date().toISOString()
    };

    service.createEvent(reqBody).subscribe(r => expect(r).toEqual(resp));

    const req = httpMock.expectOne(`${eventsApi}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(reqBody);
    req.flush(resp);
  });

  it('updateEvent should put and return EventResponse', () => {
    const reqBody: EventRequest = {
      organizerId: 'org1',
      slug: 'slug',
      name: 'Name',
      description: 'Desc',
      status: 'Published'
    };
    const resp: EventResponse = {
      id: 'e1',
      organizerId: 'org1',
      slug: 'slug',
      name: 'Name',
      description: 'Desc',
      status: 'Published',
      createdAtUtc: new Date().toISOString(),
      updatedAtUtc: new Date().toISOString()
    };

    service.updateEvent(reqBody, 'e1').subscribe(r => expect(r).toEqual(resp));

    const req = httpMock.expectOne(`${eventsApi}/e1`);
    expect(req.request.method).toBe('PUT');
    req.flush(resp);
  });

  it('getEventById should GET event', () => {
    const e: SeatifyEvent = {
      id: 'e1',
      organizerId: 'org1',
      slug: 's',
      name: 'n',
      description: 'd',
      status: 'Published',
      createdAtUtc: new Date().toISOString(),
      updatedAtUtc: new Date().toISOString()
    };

    service.getEventById('e1').subscribe(res => expect(res).toEqual(e));

    const req = httpMock.expectOne(`${apiUrl}/events/e1`);
    expect(req.request.method).toBe('GET');
    req.flush(e);
  });

  it('getEventBySlug should GET public slug endpoint', () => {
    const e: SeatifyEvent = {
      id: 'e1',
      organizerId: 'org1',
      slug: 'myslug',
      name: 'n',
      description: 'd',
      status: 'Published',
      createdAtUtc: new Date().toISOString(),
      updatedAtUtc: new Date().toISOString()
    };

    service.getEventBySlug('myslug').subscribe(res => expect(res).toEqual(e));

    const req = httpMock.expectOne(`${apiUrl}/events/public/slug/myslug`);
    expect(req.request.method).toBe('GET');
    req.flush(e);
  });

  it('checkEventHasBookings should GET boolean', () => {
    service.checkEventHasBookings('e1').subscribe(res => expect(res).toBeTrue());

    const req = httpMock.expectOne(`${eventsApi}/e1/has-bookings`);
    expect(req.request.method).toBe('GET');
    req.flush(true);
  });

  it('checkOccurrenceHasBookings should map hasBookings from occurrence', () => {
    const occ: EventOccurrence = {
      id: 'o1',
      eventId: 'e1',
      venueId: 'v1',
      auditoriumId: 'a1',
      startsAtUtc: new Date().toISOString(),
      endsAtUtc: new Date().toISOString(),
      bookingOpenAtUtc: new Date().toISOString(),
      bookingCloseAtUtc: new Date().toISOString(),
      status: 'Scheduled',
      effectiveCurrency: 'USD',
      hasBookings: true
    };

    service.checkOccurrenceHasBookings('o1').subscribe(res => expect(res).toBeTrue());

    const req = httpMock.expectOne(`${occurrencesApi}/o1`);
    expect(req.request.method).toBe('GET');
    req.flush(occ);
  });

  it('getOccurrencesByEventId should return occurrences or [] on null', () => {
    const occs: EventOccurrence[] = [{
      id: 'o1',
      eventId: 'e1',
      venueId: 'v1',
      auditoriumId: 'a1',
      startsAtUtc: '2026-01-01T00:00:00Z',
      endsAtUtc: '2026-01-01T02:00:00Z',
      bookingOpenAtUtc: '2025-12-01T00:00:00Z',
      bookingCloseAtUtc: '2025-12-31T00:00:00Z',
      status: 'Scheduled',
      effectiveCurrency: 'USD'
    }];

    service.getOccurrencesByEventId('e1').subscribe(res => expect(res).toEqual(occs));

    const req = httpMock.expectOne(`${occurrencesApi}/by-event/e1`);
    expect(req.request.method).toBe('GET');
    req.flush(occs);

    service.getOccurrencesByEventId('e2').subscribe(res => expect(res).toEqual([]));
    const req2 = httpMock.expectOne(`${occurrencesApi}/by-event/e2`);
    req2.flush(null);
  });

  it('createOccurrence and updateOccurrence should POST/PUT', () => {
    const partial = { eventId: 'e1' };

    service.createOccurrence(partial).subscribe(res => expect(res).toEqual({ ok: true }));
    const r1 = httpMock.expectOne(`${occurrencesApi}`);
    expect(r1.request.method).toBe('POST');
    r1.flush({ ok: true });

    service.updateOccurrence('o1', partial).subscribe(res => expect(res).toEqual({ ok: true }));
    const r2 = httpMock.expectOne(`${occurrencesApi}/o1`);
    expect(r2.request.method).toBe('PUT');
    r2.flush({ ok: true });
  });

  it('getEventCards should return sorted EventCard[] and handle empty events', (done) => {
    const events: SeatifyEvent[] = [
      { id: 'e1', organizerId: 'org1', slug: 's1', name: 'Name1', description: 'd1', status: 'Published', createdAtUtc: '', updatedAtUtc: '' },
      { id: 'e2', organizerId: 'org1', slug: 's2', name: 'Name2', description: 'd2', status: 'Draft', createdAtUtc: '', updatedAtUtc: '' }
    ];

    const occsE1: EventOccurrence[] = [{
      id: 'o1',
      eventId: 'e1',
      venueId: 'v1',
      auditoriumId: 'a1',
      startsAtUtc: '2026-06-01T10:00:00Z',
      endsAtUtc: '2026-06-01T12:00:00Z',
      bookingOpenAtUtc: '',
      bookingCloseAtUtc: '',
      status: 'Scheduled',
      effectiveCurrency: 'USD',
      venue: { id: 'v1', name: 'Venue 1' },
      auditorium: { id: 'a1', name: 'Aud 1' }
    }];

    const occsE2: EventOccurrence[] = [{
      id: 'o2',
      eventId: 'e2',
      venueId: 'v1',
      auditoriumId: 'a2',
      startsAtUtc: '2026-05-01T10:00:00Z',
      endsAtUtc: '2026-05-01T12:00:00Z',
      bookingOpenAtUtc: '',
      bookingCloseAtUtc: '',
      status: 'Scheduled',
      effectiveCurrency: 'USD',
      venue: { id: 'v1', name: 'Venue 1' },
      auditorium: { id: 'a2', name: 'Aud 2' }
    }];

    service.getEventCards('org1').subscribe(cards => {
      expect(Array.isArray(cards)).toBeTrue();
      expect(cards.length).toBe(2);
      expect(cards[0].id).toBe('e2');
      expect(cards[0].venueName).toBe('Venue 1');
      expect(cards[0].auditoriumName).toBe('Aud 2');
      expect(cards[1].auditoriumName).toBe('Aud 1');
      done();
    });

    const reqEvents = httpMock.expectOne(`${eventsApi}/organizers/org1`);
    expect(reqEvents.request.method).toBe('GET');
    reqEvents.flush(events);

    const reqOccsE1 = httpMock.expectOne(`${occurrencesApi}/by-event/e1`);
    expect(reqOccsE1.request.method).toBe('GET');
    reqOccsE1.flush(occsE1);

    const reqOccsE2 = httpMock.expectOne(`${occurrencesApi}/by-event/e2`);
    expect(reqOccsE2.request.method).toBe('GET');
    reqOccsE2.flush(occsE2);
  });

  it('getEventCards should return [] when organizer has no events', (done) => {
    service.getEventCards('org-empty').subscribe(cards => {
      expect(cards).toEqual([]);
      done();
    });

    const req = httpMock.expectOne(`${eventsApi}/organizers/org-empty`);
    req.flush([]);
  });

  it('getEventCards should handle occurrence fetch error and still produce card with no occurrences', (done) => {
    const events: SeatifyEvent[] = [
      { id: 'e1', organizerId: 'org1', slug: 's1', name: 'Name1', description: 'd1', status: 'Published', createdAtUtc: '', updatedAtUtc: '' }
    ];

    service.getEventCards('org1').subscribe(cards => {
      expect(cards.length).toBe(1);
      expect(cards[0].occurrences.length).toBe(0);
      expect(cards[0].venueName).toBe('No venue assigned');
      done();
    });

    const reqEvents = httpMock.expectOne(`${eventsApi}/organizers/org1`);
    reqEvents.flush(events);

    const reqOcc = httpMock.expectOne(`${occurrencesApi}/by-event/e1`);
    reqOcc.flush('error', { status: 500, statusText: 'Server Error' });
  });

  it('getEvents should map null to [] and propagate fatal errors via handleFatalError', (done) => {
    service.getEvents('org1').subscribe(events => {
      expect(events).toEqual([]);
    });

    const req = httpMock.expectOne(`${eventsApi}/organizers/org1`);
    req.flush(null);

    service.getEvents('org2').subscribe({
      next: () => fail('should error'),
      error: err => {
        expect(err).toBeTruthy();
        done();
      }
    });

    const req2 = httpMock.expectOne(`${eventsApi}/organizers/org2`);
    req2.flush('fail', { status: 500, statusText: 'Server Error' });
  });

  it('getActiveEventsCount and getAllEventsCount should compute counts', () => {
    const events: SeatifyEvent[] = [
      { id: 'e1', organizerId: 'org1', slug: 's1', name: 'n1', description: '', status: 'Published', createdAtUtc: '', updatedAtUtc: '' },
      { id: 'e2', organizerId: 'org1', slug: 's2', name: 'n2', description: '', status: 'Draft', createdAtUtc: '', updatedAtUtc: '' },
      { id: 'e3', organizerId: 'org1', slug: 's3', name: 'n3', description: '', status: 'Published', createdAtUtc: '', updatedAtUtc: '' }
    ];

    service.getActiveEventsCount('org1').subscribe(count => expect(count).toBe(2));
    service.getAllEventsCount('org1').subscribe(count => expect(count).toBe(3));

    const reqs = httpMock.match(`${eventsApi}/organizers/org1`);
    expect(reqs.length).toBe(2);
    reqs.forEach(r => r.flush(events));
  });

  it('handleFatalError logs and throws Error', () => {
    const err = new HttpErrorResponse({ status: 500, statusText: 'Err' });
    let thrown = false;
    (service as any).handleFatalError(err).subscribe({
      next: () => fail('should not next'),
      error: (e: any) => {
        expect(e).toBeTruthy();
        thrown = true;
      }
    });
    expect(thrown).toBeTrue();
  });
});
