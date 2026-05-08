import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Appearance, AppearanceCreateRequest } from '../models/appearance';

@Injectable({
  providedIn: 'root'
})
export class AppearanceService {
  private readonly apiUrl = `${environment.baseApiUrl}/api/appearance`;

  constructor(private readonly http: HttpClient) {}

  getMyAppearances(): Observable<Appearance[]> {
    return this.http.get<Appearance[]>(this.apiUrl);
  }

  getById(id: string): Observable<Appearance> {
    return this.http.get<Appearance>(`${this.apiUrl}/${id}`);
  }

  create(request: AppearanceCreateRequest): Observable<Appearance> {
    return this.http.post<Appearance>(this.apiUrl, request);
  }

  update(id: string, request: AppearanceCreateRequest): Observable<Appearance> {
    return this.http.put<Appearance>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  setDefault(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/default`, {});
  }
}
