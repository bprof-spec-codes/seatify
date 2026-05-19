import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { ConfigService } from './config.service';
import { Config } from '../models/config';

describe('ConfigService', () => {
  let service: ConfigService;
  let httpMock: HttpTestingController;

  const mockConfig: Config = {
    baseApiUrl: 'http://localhost:5141'
  } as Config;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ConfigService]
    });

    service = TestBed.inject(ConfigService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should load config from config.json and store it in cfg', () => {
    service.load().subscribe(config => {
      expect(config).toEqual(mockConfig);
      expect(service.cfg).toEqual(mockConfig);
    });

    const req = httpMock.expectOne('/config.json');

    expect(req.request.method).toBe('GET');
    expect(req.request.headers.get('Cache-Control')).toBe('no-cache');

    req.flush(mockConfig);
  });

  it('should return apiBaseUrl without trailing slash', () => {
    service.cfg = {
      baseApiUrl: 'http://localhost:5141/'
    } as Config;

    expect(service.apiBaseUrl).toBe('http://localhost:5141');
  });

  it('should return apiBaseUrl unchanged if it has no trailing slash', () => {
    service.cfg = {
      baseApiUrl: 'http://localhost:5141'
    } as Config;

    expect(service.apiBaseUrl).toBe('http://localhost:5141');
  });
});