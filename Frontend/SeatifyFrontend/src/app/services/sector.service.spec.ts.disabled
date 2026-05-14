import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SectorService } from './sector.service';
import { Sector, SectorViewDto, CreateUpdateSectorDto } from '../models/sector';
import { environment } from '../../environments/environment';

describe('SectorService', () => {
  let service: SectorService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.baseApiUrl}/api`;

  // Mock data
  const mockSectorDto: SectorViewDto = {
    id: '1',
    auditoriumId: 'aud-123',
    name: 'VIP Sector',
    color: '#FF0000',
    basePrice: 5000,
    createdAtUtc: '2024-05-01T10:00:00Z',
    updatedAtUtc: '2024-05-01T12:00:00Z'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SectorService]
    });
    service = TestBed.inject(SectorService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getSectorsByAuditoriumId', () => {
    it('should fetch sectors and update sector$ stream', () => {
      const auditoriumId = 'aud-123';
      const mockResponse: Sector[] = [
        { ...new Sector(), id: '1', createdAtUtc: new Date(), updatedAtUtc: new Date() }
      ];

      service.getSectorsByAuditoriumId(auditoriumId).subscribe(sectors => {
        expect(sectors.length).toBe(1);
        expect(sectors[0].id).toBe('1');
      });

      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/sectors`);
      expect(req.request.method).toBe('GET');
      req.flush(mockResponse);

      service.sector$.subscribe(currentSectors => {
        expect(currentSectors.length).toBe(1);
      });
    });
  });

  describe('createSector', () => {
    it('should post new sector and add to the list', () => {
      const auditoriumId = 'aud-123';
      const dto: CreateUpdateSectorDto = { name: 'New', color: '#000', basePrice: 100 };
      
      service.clearSectors();

      service.createSector(auditoriumId, dto).subscribe(result => {
        expect(result.id).toBe('1');
        expect(result.createdAtUtc).toBeInstanceOf(Date);
      });

      const req = httpMock.expectOne(`${apiUrl}/auditoriums/${auditoriumId}/sectors`);
      expect(req.request.method).toBe('POST');
      req.flush(mockSectorDto);

      service.sector$.subscribe(list => {
        expect(list.length).toBe(1);
        expect(list[0].name).toBe('VIP Sector');
      });
    });
  });

  describe('updateSector', () => {
    it('should put updated data and refresh the specific sector in the list', () => {
      const sectorId = '1';
      const updateDto: CreateUpdateSectorDto = { name: 'Updated Name', color: '#FFF', basePrice: 200 };
      
      const initialSector = new Sector();
      initialSector.id = '1';
      initialSector.name = 'Old Name';
      service.setSectors([initialSector]);

      service.updateSector(sectorId, updateDto).subscribe(updated => {
        expect(updated.name).toBe('VIP Sector');
      });

      const req = httpMock.expectOne(`${apiUrl}/sectors/${sectorId}`);
      expect(req.request.method).toBe('PUT');
      req.flush(mockSectorDto);

      service.sector$.subscribe(list => {
        expect(list[0].name).toBe('VIP Sector');
      });
    });
  });

  describe('deleteSector', () => {
    it('should send delete request and remove from local list', () => {
      const sectorId = '1';
      const initialSector = new Sector();
      initialSector.id = '1';
      service.setSectors([initialSector]);

      service.deleteSector(sectorId).subscribe();

      const req = httpMock.expectOne(`${apiUrl}/sectors/${sectorId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);

      service.sector$.subscribe(list => {
        expect(list.length).toBe(0);
      });
    });
  });

  describe('Error Handling', () => {
    it('should handle 500 error gracefully', () => {
      service.getSectorById('999').subscribe({
        next: () => fail('Should have failed with 500 error'),
        error: (error) => {
          expect(error.message).toContain('Something went wrong');
        }
      });

      const req = httpMock.expectOne(`${apiUrl}/sectors/999`);
      req.flush('Internal Server Error', { status: 500, statusText: 'Server Error' });
    });
  });
});
