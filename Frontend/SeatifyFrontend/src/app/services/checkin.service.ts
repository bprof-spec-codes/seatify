import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ConfigService } from './config.service';

export interface CheckInValidateRequest {
  payload: string;
}

export interface CheckInConfirmRequest {
  ticketId: string;
}

export enum TicketStatus {
  Valid = 0,
  AlreadyUsed = 1,
  Invalid = 2
}

export interface ReservationInfo {
  customerName: string;
  customerEmail: string;
}

export interface SeatInfo {
  section: string;
  row: number;
  number: number;
  seatLabel?: string;
}

export interface EventInfo {
  title: string;
  startTime: string;
}

export interface CheckInResult {
  ticketId: string;
  status: TicketStatus;
  statusMessage: string;
  reservation?: ReservationInfo;
  seat?: SeatInfo;
  event?: EventInfo;
}

@Injectable({
  providedIn: 'root'
})
export class CheckInService {

  private apiUrl = `${environment.baseApiUrl}/api/checkin`;

  private readonly checkInPath = '/api/checkin';

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) { }

  private api(path: string): string {
    return `${this.configService.cfg.baseApiUrl}${path}`;
  }

  validateTicket(payload: string): Observable<CheckInResult> {
    return this.http.post<CheckInResult>(`${this.api(this.checkInPath)}/validate`, { payload });
  }

  confirmCheckIn(ticketId: string): Observable<CheckInResult> {
    return this.http.post<CheckInResult>(`${this.api(this.checkInPath)}/confirm`, { ticketId });
  }
}
