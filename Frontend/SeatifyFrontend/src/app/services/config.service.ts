import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Config } from '../models/config';
@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  public cfg: Config = new Config();

  constructor(private readonly http: HttpClient) { }

  load() {
    return this.http.get<Config>('/config.json', {
      headers: {
        'Cache-Control': 'no-cache'
      }
    }).pipe(
      tap(cfg => this.cfg = cfg)
    )
  }

  get apiBaseUrl(): string {
    return this.cfg.baseApiUrl.replace(/\/$/, '')
  }
}
