import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Appearance, AppearanceCreateRequest } from '../models/appearance';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class AppearanceService {
  private readonly apiUrl = `${environment.baseApiUrl}/api/appearance`;

  constructor(
    private readonly http: HttpClient,
    private readonly configService: ConfigService
  ) { }

  getMyAppearances(): Observable<Appearance[]> {
    return this.http.get<Appearance[]>(this.configService.apiBaseUrl + '/api/appearance');
  }

  getById(id: string): Observable<Appearance> {
    return this.http.get<Appearance>(`${this.configService.apiBaseUrl}/api/appearance/${id}`);
  }

  create(request: AppearanceCreateRequest): Observable<Appearance> {
    return this.http.post<Appearance>(this.configService.apiBaseUrl + '/api/appearance', request);
  }

  update(id: string, request: AppearanceCreateRequest): Observable<Appearance> {
    return this.http.put<Appearance>(`${this.configService.apiBaseUrl}/api/appearance/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.configService.apiBaseUrl}/api/appearance/${id}`);
  }

  setDefault(id: string): Observable<void> {
    return this.http.post<void>(`${this.configService.apiBaseUrl}/api/appearance/${id}/default`, {});
  }
}
