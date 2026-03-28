import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from 'rxjs';
import { CreateLayoutMatrixDto, LayoutMatrix } from '../models/layout-matrix';
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

  createLayoutMatrix(dto: CreateLayoutMatrixDto, auditoriumId: string): Observable<LayoutMatrix> {
    return this.http.post<LayoutMatrix>(`${this.apiUrl}/auditoriums/${auditoriumId}/layout-matrices`, dto).pipe(
      map(createdMatrix => this.mapLayoutMatrixDates(createdMatrix)),
      catchError(this.handleError)
    )
  }

  updateLayoutMatrix(matrixId: string, dto: CreateLayoutMatrixDto): Observable<LayoutMatrix> {
    return this.http.put<LayoutMatrix>(`${this.apiUrl}/layout-matrices/${matrixId}`, dto).pipe(
      map(updatedMatrix => this.mapLayoutMatrixDates(updatedMatrix)),
      catchError(this.handleError)
    )
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
