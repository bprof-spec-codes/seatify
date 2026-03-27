import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from 'rxjs';
import { LayoutMatrix } from '../../models/layout-matrix';
import { ActivatedRoute } from '@angular/router';
import { MatrixCellVm } from '../../models/matrix-cell-vm';
import { SeatService } from '../../services/seat.service';
import { SeatType } from '../../models/seat';
import { SeatMap } from '../../models/seat-map';

@Component({
  selector: 'app-layout-matrix-editor',
  standalone: false,
  templateUrl: './layout-matrix-editor.component.html',
  styleUrl: './layout-matrix-editor.component.sass',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LayoutMatrixEditorComponent implements OnInit {
  matrices$!: Observable<LayoutMatrix[]>
  selectedMatrix: LayoutMatrix | null = null

  private selectedMatrixSubject = new BehaviorSubject<LayoutMatrix | null>(null);
  selectedMatrix$ = this.selectedMatrixSubject.asObservable();
  seatMap$!: Observable<SeatMap | null>;

  gridCells: MatrixCellVm[] = []
  selectedCellKey: string | null = null

  auditoriumId: string = ""
  seatType = SeatType

  gridRows = 0;
  gridColumns = 0;

  constructor(
    private matrixService: LayoutMatrixService,
    private route: ActivatedRoute,
    private seatService: SeatService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.auditoriumId = this.route.snapshot.paramMap.get('auditoriumId') ?? 'aud-id-01';
    this.matrices$ = this.matrixService.LayoutMatrix$

    this.seatMap$ = this.selectedMatrixSubject.pipe(
      tap(() => {
        this.gridCells = []
        this.gridRows = 0
        this.gridColumns = 0
        this.selectedCellKey = null
        this.cdr.markForCheck()
      }),
      switchMap(matrix => {
        if (!matrix) {
          return of(null)
        }

        return this.seatService.getSeatMapByMatrixId(matrix.id).pipe(
          catchError(err => {
            console.error('Failed to load seat map', err)
            return of(null)
          })
        )
      }),
      tap(seatMap => {
        if (!seatMap) {
          this.gridCells = []
          this.gridRows = 0
          this.gridColumns = 0
        } else {
          this.gridRows = seatMap.rows
          this.gridColumns = seatMap.columns
          this.gridCells = this.buildGridCellsFromSeatMap(seatMap)
        }

        this.cdr.markForCheck()
      })
    )

    this.seatMap$.subscribe()
    this.loadMatrices()
  }

  loadMatrices(): void {
    this.matrixService.getLayoutMatrixByAuditoriumId(this.auditoriumId).subscribe({
      next: matrices => {
        if (matrices.length === 0) {
          this.setSelectedMatrix(null)
          this.gridCells = []
          this.seatService.clearSeatMap()
          return
        }

        if (this.selectedMatrix) {
          const stillExists = matrices.find(m => m.id === this.selectedMatrix?.id)
          this.selectMatrix(stillExists ?? matrices[0])
          return
        }

        this.selectMatrix(matrices[0])
      },
      error: err => {
        console.error('Failed to load layout matrices', err)
        this.setSelectedMatrix(null)
        this.gridCells = []
        this.seatService.clearSeatMap()
      }
    })
  }

  private loadSeatMap(matrixId: string): void {
    this.gridCells = []
    this.gridRows = 0
    this.gridColumns = 0
    this.cdr.markForCheck()

    this.seatService.getSeatMapByMatrixId(matrixId).subscribe({
      next: seatMap => {
        if (this.selectedMatrix?.id !== matrixId) {
          return
        }

        this.gridRows = seatMap.rows
        this.gridColumns = seatMap.columns
        this.gridCells = this.buildGridCellsFromSeatMap(seatMap);
        this.cdr.markForCheck();
      },
      error: err => {
        console.error('Failed to load seat map', err)

        if (this.selectedMatrix?.id === matrixId) {
          this.gridCells = []
          this.gridRows = 0
          this.gridColumns = 0
          this.cdr.markForCheck()
        }
      }
    });
  }

  selectMatrix(matrix: LayoutMatrix): void {
    this.setSelectedMatrix(matrix)
  }

  isSelected(matrix: LayoutMatrix): boolean {
    return this.selectedMatrix?.id === matrix.id
  }

  private setSelectedMatrix(matrix: LayoutMatrix | null): void {
    this.selectedMatrix = matrix
    this.selectedMatrixSubject.next(matrix)
    this.selectedCellKey = null
    this.cdr.markForCheck()
  }

  private buildGridCellsFromSeatMap(seatMap: SeatMap): MatrixCellVm[] {
    const cells: MatrixCellVm[] = [];
    const seatLookup = new Map<string, typeof seatMap.seats[number]>();

    for (const seat of seatMap.seats) {
      seatLookup.set(`${seat.row}-${seat.column}`, seat);
    }

    for (let row = 1; row <= seatMap.rows; row++) {
      for (let column = 1; column <= seatMap.columns; column++) {
        const key = `${row}-${column}`;
        const seat = seatLookup.get(key);

        cells.push({
          key,
          row,
          column,
          seatId: seat?.id ?? null,
          seatLabel: seat?.seatLabel ?? null,
          seatType: seat?.seatType ?? SeatType.Seat,
          sectorId: seat?.sectorId ?? null,
          priceOverride: seat?.priceOverride ?? null
        });
      }
    }

    console.log('selectedMatrix', this.selectedMatrix);
    console.log('seatMap dims', seatMap.rows, seatMap.columns);
    console.log('gridCells length', cells.length);

    return cells;
  }

  trackByMatrixId(index: number, matrix: LayoutMatrix): string {
    return matrix.id
  }

  trackByCellKey(index: number, cell: MatrixCellVm): string {
    return cell.key
  }

  get selectedMatrixTitle(): string {
    return this.selectedMatrix?.name ?? 'No matrix selected'
  }

  get selectedMatrixSummary(): string {
    if (!this.selectedMatrix) return ''
    return `${this.selectedMatrix.rows} rows × ${this.selectedMatrix.columns} columns`
  }

  selectCell(cell: MatrixCellVm): void {
    this.selectedCellKey = cell.key
  }

  isCellSelected(cell: MatrixCellVm): boolean {
    return this.selectedCellKey === cell.key
  }

  get selectedCell(): MatrixCellVm | null {
    if (!this.selectedCellKey) return null
    return this.gridCells.find(c => c.key === this.selectedCellKey) ?? null
  }

  setSelectedCellSeatType(type: SeatType): void {
    if (!this.selectedCell) return

    const key = this.selectedCell.key

    this.gridCells = this.gridCells.map(c =>
      c.key === key ? { ...c, seatType: type } : c
    )

    this.cdr.markForCheck()
  }

  getSeatTypeLabel(type: SeatType): string {
    switch (type) {
      case SeatType.Seat: return 'Standard';
      case SeatType.AccessibleSeat: return 'Accessible';
      case SeatType.Aisle: return 'Aisle';
      default: return 'Unknown';
    }
  }

}
