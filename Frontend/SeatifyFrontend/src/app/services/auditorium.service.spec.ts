import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuditoriumService } from './auditorium.service';
import { environment } from '../../environments/environment';
import { Auditorium } from '../models/auditorium';

describe('AuditoriumService', () => {
  let service: AuditoriumService;
  let httpMock: HttpTestingController;
  const baseApi = environment.baseApiUrl;
  const apiUrl = `${baseApi}/api/auditoriums`;
  const venuesApiUrl = `${baseApi}/api/venues`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuditoriumService]
    });
    service = TestBed.inject(AuditoriumService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch auditorium by id', (done) => {
    const mock: Auditorium = { id: '1', venueId: 'v1', name: 'Main', description: '', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() };
    service.getAuditoriumById('1').subscribe(res => {
      expect(res).toEqual(mock);
      done();
    });
    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush(mock);
  });

  it('should fetch auditoriums by venue id', (done) => {
    const mock: Auditorium[] = [
      { id: 'a1', venueId: 'v1', name: 'A', description: '', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() },
      { id: 'a2', venueId: 'v1', name: 'B', description: '', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() }
    ];
    service.getAuditoriumsByVenueId('v1').subscribe(res => {
      expect(res).toEqual(mock);
      done();
    });
    const req = httpMock.expectOne(`${venuesApiUrl}/v1/auditoriums`);
    expect(req.request.method).toBe('GET');
    req.flush(mock);
  });

  it('should create auditorium and update internal subject', (done) => {
    const newAud: Auditorium = { id: 'new', venueId: 'v1', name: 'New Hall', description: 'desc', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() };
    service.auditoriums$.subscribe(list => {
      if (list.length) {
        expect(list).toContain(jasmine.objectContaining({ id: 'new', name: 'New Hall' }));
        done();
      }
    });
    service.createAuditorium('v1', newAud).subscribe(res => expect(res).toEqual(newAud));
    const req = httpMock.expectOne(`${venuesApiUrl}/v1/auditoriums`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newAud);
    req.flush(newAud);
  });

  it('should update auditorium and update internal subject when exists', (done) => {
    const existing: Auditorium = { id: 'u1', venueId: 'v1', name: 'Old', description: '', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() };
    (service as any).auditoriumsSource$.next([existing]);
    const updated: Auditorium = { id: 'u1', venueId: 'v1', name: 'Updated', description: 'updated', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() };
    service.auditoriums$.subscribe(list => {
      const found = list.find(a => a.id === 'u1');
      if (found && found.name === 'Updated') {
        expect(found.description).toBe('updated');
        done();
      }
    });
    service.updateAuditorium(updated).subscribe(res => expect(res).toEqual(updated));
    const req = httpMock.expectOne(`${apiUrl}/u1`);
    expect(req.request.method).toBe('PUT');
    req.flush(updated);
  });

  it('should delete auditorium and remove from internal subject', (done) => {
    const a1: Auditorium = { id: 'd1', venueId: 'v1', name: 'D1', description: '', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() };
    const a2: Auditorium = { id: 'd2', venueId: 'v1', name: 'D2', description: '', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() };
    (service as any).auditoriumsSource$.next([a1, a2]);
    service.auditoriums$.subscribe(list => {
      if (list.length === 1) {
        expect(list.find(a => a.id === 'd1')).toBeUndefined();
        expect(list[0].id).toBe('d2');
        done();
      }
    });
    service.deleteAuditoriumById('d1').subscribe(() => {});
    const req = httpMock.expectOne(`${apiUrl}/d1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('getEditMode / setEditMode should update editMode$ observable', (done) => {
    service.getEditMode().subscribe(mode => {
      if (mode === true) { expect(mode).toBeTrue(); done(); }
    });
    service.setEditMode(true);
  });

  it('getEditAuditorium / setEditAuditorium should update editAuditorium$ observable', (done) => {
    const aud: Auditorium = { id: 'e1', venueId: 'v1', name: 'Not Main', description: '', currency: 'EUR', createdAtUtc: new Date(), updatedAtUtc: new Date() };
    service.getEditAuditorium().subscribe(current => {
      if (current?.id === 'e1') { expect(current).toEqual(aud); done(); }
    });
    service.setEditAuditorium(aud);
  });

  it('checkAuditoriumHasBookings should return boolean from backend', (done) => {
    service.checkAuditoriumHasBookings('b1').subscribe(res => { expect(res).toBeTrue(); done(); });
    const req = httpMock.expectOne(`${apiUrl}/b1/has-bookings`);
    expect(req.request.method).toBe('GET');
    req.flush(true);
  });

  it('should handle HTTP error and return user-friendly error via observable error callback', (done) => {
    service.getAuditoriumById('err').subscribe({
      next: () => fail('expected error'),
      error: (err: Error) => { expect(err.message).toContain('Something went wrong'); done(); }
    });
    const req = httpMock.expectOne(`${apiUrl}/err`);
    expect(req.request.method).toBe('GET');
    req.flush({ message: 'Server error' }, { status: 500, statusText: 'Server Error' });
  });
});
