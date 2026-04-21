import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, tap, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import {AuthResponse, CurrentUser, LoginRequest, RegisterRequest} from '../models/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.baseApiUrl}/api/auth`;
  private readonly tokenKey = 'seatify_token';
  private readonly currentUserKey = 'seatify_current_user';

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
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.currentUserKey);
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getCurrentUserSnapshot(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  private storeAuth(response: AuthResponse): void {
    localStorage.setItem(this.tokenKey, response.token);

    const user: CurrentUser = {
      organizerId: response.organizerId,
      email: response.email,
      name: response.name
    };

    localStorage.setItem(this.currentUserKey, JSON.stringify(user));
    this.currentUserSubject.next(user);
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
    const message =
      error.error?.message ||
      error.error?.errorMessage ||
      'Unexpected error occurred.';

    return throwError(() => new Error(message));
  }
}
