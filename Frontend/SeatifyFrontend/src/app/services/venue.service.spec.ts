import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { VenueService } from './venue.service';
import { Venue } from '../models/venue';
import { environment } from '../../environments/environment';

describe('VenueService', () => {
  let service: VenueService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.baseApiUrl}/api/venues`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [VenueService]
    });
    service = TestBed.inject(VenueService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getVenueById', () => {
    it('kellene hívnia a GET metódust és frissítenie a venuesSource-t', () => {
      const mockVenue: Venue = { 
        id: '1', name: 'Test Venue', city: 'Budapest', 
        postalCode: '1011', addressLine: 'Fő utca 1', 
        auditoriums: [], organizerId: 'org1' 
      };

      service.getVenueById('1').subscribe(venue => {
        expect(venue).toEqual(mockVenue);
      });

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockVenue);

      service.venues$.subscribe(venues => {
        expect(venues).toContain(mockVenue);
      });
    });
  });

  describe('createVenue', () => {
    it('kellene küldenie egy POST kérést és hozzáadnia az új helyszínt a listához', () => {
      const newVenue: Venue = { id: '2', name: 'New Venue' } as Venue;

      service.createVenue(newVenue).subscribe(venue => {
        expect(venue).toEqual(newVenue);
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('POST');
      req.flush(newVenue);

      service.venues$.subscribe(venues => {
        expect(venues.find(v => v.id === '2')).toBeTruthy();
      });
    });
  });

  describe('updateVenue', () => {
    it('kellene küldenie egy PUT kérést és frissítenie a meglévő helyszínt', () => {
      const initialVenue: Venue = { id: '1', name: 'Old Name' } as Venue;
      const updatedVenue: Venue = { id: '1', name: 'Updated Name' } as Venue;

      (service as any).venuesSource.next([initialVenue]);

      service.updateVenue(updatedVenue).subscribe();

      const req = httpMock.expectOne(`${apiUrl}/1`);
      expect(req.request.method).toBe('PUT');
      req.flush(updatedVenue);

      service.venues$.subscribe(venues => {
        expect(venues[0].name).toBe('Updated Name');
      });
    });
  });

  describe('deleteVenueById', () => {
    it('kellene küldenie egy DELETE kérést és eltávolítania a helyszínt a listából', () => {
      const venueToDelete: Venue = { id: '3', name: 'Delete Me' } as Venue;
      (service as any).venuesSource.next([venueToDelete]);

      service.deleteVenueById('3').subscribe();

      const req = httpMock.expectOne(`${apiUrl}/3`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);

      service.venues$.subscribe(venues => {
        expect(venues.length).toBe(0);
      });
    });
  });

  describe('Hiba kezelés', () => {
    it('vissza kellene adnia egy hibaüzenetet, ha a szerver hibát küld', () => {
      service.getVenueById('err').subscribe({
        next: () => fail('Sikerágra futott hiba helyett'),
        error: (error) => {
          expect(error.message).toBe('Something went wrong; please try again later.');
        }
      });

      const req = httpMock.expectOne(`${apiUrl}/err`);
      req.error(new ErrorEvent('Network error'));
    });
  });
});
