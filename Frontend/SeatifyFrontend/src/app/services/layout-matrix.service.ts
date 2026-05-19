import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, catchError, map, Observable, tap, throwError } from 'rxjs';
import { CreateLayoutMatrixDto, LayoutMatrix } from '../models/layout-matrix';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class LayoutMatrixService {
  private apiUrl = `${environment.baseApiUrl}/api`

  private readonly layoutMatricesPath = '/api';

  private LayoutMatrixSource = new BehaviorSubject<LayoutMatrix[]>([])
  LayoutMatrix$ = this.LayoutMatrixSource.asObservable()

  constructor(
    private http: HttpClient,
    private configService: ConfigService
  ) { }

  private api(path: string): string {
    return `${this.configService.cfg.baseApiUrl}${path}`;
  }

  getLayoutMatrixByAuditoriumId(auditoriumId: string): Observable<LayoutMatrix[]> {
        return this.http.get<LayoutMatrix[]>(`${this.api(this.layoutMatricesPath)}/auditoriums/${auditoriumId}/layout-matrices`).pipe(
      map(matrices => matrices.map(matrix => this.mapLayoutMatrixDates(matrix))),
      tap(matrices => this.LayoutMatrixSource.next(matrices)),
      catchError(this.handleError)
    );
  }

  createLayoutMatrix(dto: CreateLayoutMatrixDto, auditoriumId: string): Observable<LayoutMatrix> {
    return this.http.post<LayoutMatrix>(`${this.api(this.layoutMatricesPath)}/auditoriums/${auditoriumId}/layout-matrices`, dto).pipe(
      map(createdMatrix => this.mapLayoutMatrixDates(createdMatrix)),
      tap(createdMatrix => {
        const currentMatrices = this.LayoutMatrixSource.getValue();
        this.LayoutMatrixSource.next([...currentMatrices, createdMatrix]);
      }),
      catchError(this.handleError)
    )
  }

  updateLayoutMatrix(matrixId: string, dto: CreateLayoutMatrixDto): Observable<LayoutMatrix> {
    return this.http.put<LayoutMatrix>(`${this.api(this.layoutMatricesPath)}/layout-matrices/${matrixId}`, dto).pipe(
      map(updatedMatrix => this.mapLayoutMatrixDates(updatedMatrix)),
      tap(updatedMatrix => {
        const currentMatrices = this.LayoutMatrixSource.getValue();
        const index = currentMatrices.findIndex(m => m.id === matrixId);
        if (index !== -1) {
          currentMatrices[index] = updatedMatrix;
          this.LayoutMatrixSource.next([...currentMatrices]);
        }
      }),
      catchError(this.handleError)
    )
  }

  deleteLayoutMatrix(matrixId: string): Observable<void> {
    return this.http.delete<void>(`${this.api(this.layoutMatricesPath)}/layout-matrices/${matrixId}`).pipe(
      tap(() => {
        const currentMatrices = this.LayoutMatrixSource.getValue();
        const index = currentMatrices.findIndex(m => m.id === matrixId);
        if (index !== -1) {
          currentMatrices.splice(index, 1);
          this.LayoutMatrixSource.next([...currentMatrices]);
        }
      }),
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
