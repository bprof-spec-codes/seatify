import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthResponse, CurrentUser, LoginRequest, RegisterRequest } from '../models/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.baseApiUrl}/api/auth`;
  private readonly tokenKey = 'seatify_token';
  private readonly currentUserKey = 'seatify_current_user'
  private readonly expiresAtKey = 'seatify_expires_at'

  private currentUserSubject = new BehaviorSubject<CurrentUser | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) { }

  login(dto: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, dto).pipe(
      tap(response => this.storeAuth(response)),
      catchError(error => this.handleError(error))
    );
  }

  register(dto: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, dto).pipe(
      tap(response => this.storeAuth(response)),
      catchError(error => this.handleError(error))
    );
  }

  getMe(): Observable<CurrentUser> {
    return this.http.get<CurrentUser>(`${this.apiUrl}/me`).pipe(
      tap(user => {
        localStorage.setItem(this.currentUserKey, JSON.stringify(user));
        this.currentUserSubject.next(user);
      }),
      catchError(error => this.handleError(error))
    );
  }

  loginAsDev(): Observable<AuthResponse> {
    return this.login({
      email: 'dev@seatify.hu',
      password: '123456789'
    });
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey)
    localStorage.removeItem(this.currentUserKey)
    localStorage.removeItem(this.expiresAtKey)
    this.currentUserSubject.next(null)
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken()

    if (!token) {
      return false
    }

    if (this.isTokenExpired()) {
      this.logout()
      return false
    }

    return true
  }

  getCurrentUserSnapshot(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  private storeAuth(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.token)
    localStorage.setItem(this.expiresAtKey, response.expiresAtUtc)

    const user: CurrentUser = {
      organizerId: response.organizerId,
      email: response.email,
      name: response.name
    }

    localStorage.setItem(this.currentUserKey, JSON.stringify(user))
    this.currentUserSubject.next(user)
  }

  private getStoredUser(): CurrentUser | null {
    const raw = localStorage.getItem(this.currentUserKey);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as CurrentUser;
    } catch {
      return null;
    }
  }

  private handleError(error: HttpErrorResponse) {
    console.error('Auth error:', error);
    console.error('Auth error payload:', error.error);

    const message =
      error.error?.message ||
      error.error?.errorMessage ||
      error.message ||
      'Unexpected error occurred.';

    return throwError(() => new Error(message));
  }

  private isTokenExpired(): boolean {
    const rawExpiresAt = localStorage.getItem(this.expiresAtKey)

    if (!rawExpiresAt) {
      return false
    }

    const expiresAt = new Date(rawExpiresAt).getTime()

    if (Number.isNaN(expiresAt)) {
      return true
    }

    return expiresAt <= Date.now()
  }
}
