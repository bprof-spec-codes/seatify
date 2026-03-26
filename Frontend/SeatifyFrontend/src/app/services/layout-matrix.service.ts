import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from 'rxjs';
import { LayoutMatrix } from '../models/layout-matrix';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class LayoutMatrixService {
  private apiUrl = `${environment.baseApiUrl}/api`
  private LayoutMatrixSource = new BehaviorSubject<LayoutMatrix[]>([])
  LayoutMatrix$ = this.LayoutMatrixSource.asObservable()

  constructor(private http: HttpClient) { }

  getLayoutMatrixByAuditoriumId(auditoriumId: string): Observable<LayoutMatrix[]> {
        return this.http.get<LayoutMatrix[]>(`${this.apiUrl}/auditoriums/${auditoriumId}/layout-matrices`).pipe(
      map(matrices => matrices.map(matrix => this.mapLayoutMatrixDates(matrix))),
      tap(matrices => this.LayoutMatrixSource.next(matrices)),
      catchError(this.handleError)
    );
  }

  setMatrices(matrices: LayoutMatrix[]): void {
    this.LayoutMatrixSource.next(matrices)
  }

  clearMatrices(): void {
    this.LayoutMatrixSource.next([])
  }

  private mapLayoutMatrixDates(matrix: LayoutMatrix): LayoutMatrix {
    return {
      ...matrix,
      createdAtUtc: new Date(matrix.createdAtUtc),
      updatedAtUtc: new Date(matrix.updatedAtUtc)
    }
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    console.error('An error occurred: ', error.message)
    return throwError(() => new Error('Something went wrong; please try again later.'));
  }

}
