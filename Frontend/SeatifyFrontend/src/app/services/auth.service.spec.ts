import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService } from './auth.service';
import { environment } from '../../environments/environment';
import { AuthResponse, CurrentUser, LoginRequest, RegisterRequest } from '../models/auth';
import { HttpErrorResponse } from '@angular/common/http';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.baseApiUrl}/api/auth`;
  const tokenKey = 'seatify_token';
  const currentUserKey = 'seatify_current_user';
  const expiresAtKey = 'seatify_expires_at';

  const clearStorage = () => {
    localStorage.removeItem(tokenKey);
    localStorage.removeItem(currentUserKey);
    localStorage.removeItem(expiresAtKey);
  };

  beforeEach(() => {
    clearStorage();
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    clearStorage();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('login should POST and store auth on success', () => {
    const dto: LoginRequest = { email: 'a@b.com', password: 'pw' };
    const resp: AuthResponse = {
      token: 't',
      expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
      organizerId: 'org1',
      email: 'a@b.com',
      name: 'A'
    };

    service.login(dto).subscribe(r => expect(r).toEqual(resp));

    const req = httpMock.expectOne(`${apiUrl}/login`);
    expect(req.request.method).toBe('POST');
    req.flush(resp);

    expect(localStorage.getItem(tokenKey)).toBe(resp.token);
    expect(localStorage.getItem(expiresAtKey)).toBe(resp.expiresAtUtc);
    const storedUser = JSON.parse(localStorage.getItem(currentUserKey) as string) as CurrentUser;
    expect(storedUser).toEqual({ organizerId: resp.organizerId, email: resp.email, name: resp.name });
    expect(service.getCurrentUserSnapshot()).toEqual(storedUser);
  });

  it('register should POST and store auth on success', () => {
    const dto: RegisterRequest = { name: 'A', email: 'a@b.com', password: 'pw', confirmPassword: 'pw' };
    const resp: AuthResponse = {
      token: 't2',
      expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
      organizerId: 'org2',
      email: 'a@b.com',
      name: 'A'
    };

    service.register(dto).subscribe(r => expect(r).toEqual(resp));

    const req = httpMock.expectOne(`${apiUrl}/register`);
    expect(req.request.method).toBe('POST');
    req.flush(resp);

    expect(localStorage.getItem(tokenKey)).toBe(resp.token);
    expect(JSON.parse(localStorage.getItem(currentUserKey) as string)).toEqual({
      organizerId: resp.organizerId,
      email: resp.email,
      name: resp.name
    });
  });

  it('getMe should GET, store current user and update subject', () => {
    const user: CurrentUser = { organizerId: 'o', email: 'u@e', name: 'U' };
    service.getMe().subscribe(u => expect(u).toEqual(user));

    const req = httpMock.expectOne(`${apiUrl}/me`);
    expect(req.request.method).toBe('GET');
    req.flush(user);

    expect(JSON.parse(localStorage.getItem(currentUserKey) as string)).toEqual(user);
    expect(service.getCurrentUserSnapshot()).toEqual(user);
  });

  it('loginAsDev should call login with dev credentials', () => {
    const resp: AuthResponse = {
      token: 'devtoken',
      expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
      organizerId: 'devOrg',
      email: 'dev@seatify.hu',
      name: 'Dev'
    };

    service.loginAsDev().subscribe(r => expect(r).toEqual(resp));

    const req = httpMock.expectOne(`${apiUrl}/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'dev@seatify.hu', password: '123456789' });
    req.flush(resp);
  });

  it('logout should clear storage and set current user to null', () => {
    localStorage.setItem(tokenKey, 'x');
    localStorage.setItem(currentUserKey, JSON.stringify({ organizerId: 'o', email: 'e', name: 'n' }));
    localStorage.setItem(expiresAtKey, new Date().toISOString());
    service.logout();
    expect(localStorage.getItem(tokenKey)).toBeNull();
    expect(localStorage.getItem(currentUserKey)).toBeNull();
    expect(localStorage.getItem(expiresAtKey)).toBeNull();
    expect(service.getCurrentUserSnapshot()).toBeNull();
  });

  it('getToken should return token from localStorage', () => {
    localStorage.setItem(tokenKey, 'tok123');
    expect(service.getToken()).toBe('tok123');
  });

  it('isLoggedIn should be false when no token', () => {
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('isLoggedIn should be false and logout when token expired', () => {
    const past = new Date(Date.now() - 1000).toISOString();
    localStorage.setItem(tokenKey, 'expired');
    localStorage.setItem(expiresAtKey, past);
    spyOn(service, 'logout').and.callThrough();
    expect(service.isLoggedIn()).toBeFalse();
    expect(service.logout).toHaveBeenCalled();
  });

  it('isLoggedIn should return true when token present and not expired', () => {
    const future = new Date(Date.now() + 60_000).toISOString();
    localStorage.setItem(tokenKey, 'valid');
    localStorage.setItem(expiresAtKey, future);
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('isTokenExpired should return true for invalid date stored', () => {
    localStorage.setItem(expiresAtKey, 'not-a-date');
    const expired = (service as any).isTokenExpired();
    expect(expired).toBeTrue();
  });

  it('getStoredUser should return null for missing or invalid JSON', () => {
    expect((service as any).getStoredUser()).toBeNull();
    localStorage.setItem(currentUserKey, 'invalid json');
    expect((service as any).getStoredUser()).toBeNull();
  });

  it('storeAuth should persist token, expiresAt and current user and update subject', () => {
    const resp: AuthResponse = {
      token: 'sTok',
      expiresAtUtc: new Date(Date.now() + 5000).toISOString(),
      organizerId: 'oid',
      email: 'x@y',
      name: 'X'
    };
    (service as any).storeAuth(resp);
    expect(localStorage.getItem(tokenKey)).toBe(resp.token);
    expect(localStorage.getItem(expiresAtKey)).toBe(resp.expiresAtUtc);
    expect(JSON.parse(localStorage.getItem(currentUserKey) as string)).toEqual({
      organizerId: resp.organizerId,
      email: resp.email,
      name: resp.name
    });
    expect(service.getCurrentUserSnapshot()).toEqual({
      organizerId: resp.organizerId,
      email: resp.email,
      name: resp.name
    });
  });

  it('handleError should produce Error with payload message and throw', () => {
    const httpErr = new HttpErrorResponse({
      status: 400,
      error: { message: 'bad creds' }
    });

    let thrown: any = null;
    (service as any).handleError(httpErr).subscribe({
      next: () => fail('should not next'),
      error: (err: Error) => (thrown = err)
    });

    expect(thrown).toBeTruthy();
    expect(thrown.message).toBe('bad creds');
  });

  it('handleError should use errorMessage or fallback to message', () => {
    const httpErr2 = new HttpErrorResponse({
      status: 500,
      error: { errorMessage: 'server issue' }
    });

    let thrown2: any = null;
    (service as any).handleError(httpErr2).subscribe({
      next: () => fail('should not next'),
      error: (err: Error) => (thrown2 = err)
    });

    expect(thrown2.message).toBe('server issue');

    const httpErr3 = new HttpErrorResponse({ status: 0, error: null, statusText: 'Unknown Error' });
    let thrown3: any = null;
    (service as any).handleError(httpErr3).subscribe({
      next: () => fail('should not next'),
      error: (err: Error) => (thrown3 = err)
    });
    expect(thrown3.message).toBe(httpErr3.message || 'Unexpected error occurred.');
  });
});
