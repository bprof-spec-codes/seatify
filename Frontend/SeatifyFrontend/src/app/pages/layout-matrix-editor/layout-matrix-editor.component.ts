import { ChangeDetectionStrategy, ChangeDetectorRef, Component, ElementRef, HostListener, OnInit } from '@angular/core';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from 'rxjs';
import { CreateLayoutMatrixDto, LayoutMatrix } from '../../models/layout-matrix';
import { ActivatedRoute } from '@angular/router';
import { MatrixCellVm } from '../../models/matrix-cell-vm';
import { SeatService } from '../../services/seat.service';
import { SeatType, UpdateSeatDto } from '../../models/seat';
import { SeatMap } from '../../models/seat-map';
import { Sector } from '../../models/sector';
import { SectorService } from '../../services/sector.service';

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

  gridRows = 0
  gridColumns = 0

  isCreateFormOpen = false
  isCreatingMatrix = false
  editingMatrixId: string | null = null
  isSavingMatrix = false
  isDeletingMatrix = false

  openMatrixMenuId: string | null = null

  seatEditModel: UpdateSeatDto | null = null
  isSavingSeat = false

  sectors$!: Observable<Sector[]>;

  constructor(
    private matrixService: LayoutMatrixService,
    private route: ActivatedRoute,
    private seatService: SeatService,
    private cdr: ChangeDetectorRef,
    private elementRef: ElementRef,
    private sectorService: SectorService
  ) { }

  ngOnInit(): void {
    this.auditoriumId = this.route.snapshot.paramMap.get('auditoriumId') ?? 'aud-id-01';
    this.matrices$ = this.matrixService.LayoutMatrix$
    this.sectors$ = this.sectorService.sector$;

    this.sectorService.getSectorsByAuditoriumId(this.auditoriumId).subscribe()

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

    this.seatEditModel = {
      seatLabel: cell.seatLabel ?? '',
      sectorId: cell.sectorId ?? null,
      priceOverride: cell.priceOverride ?? null,
      seatType: cell.seatType
    }

    this.cdr.markForCheck()
  }

  isCellSelected(cell: MatrixCellVm): boolean {
    return this.selectedCellKey === cell.key
  }

  get selectedCell(): MatrixCellVm | null {
    if (!this.selectedCellKey) return null
    return this.gridCells.find(c => c.key === this.selectedCellKey) ?? null
  }

  setSelectedCellSeatType(type: SeatType): void {
    if (!this.seatEditModel) return

    this.seatEditModel = {
      ...this.seatEditModel,
      seatType: type
    }

    this.cdr.markForCheck()
  }

  getSeatTypeLabel(type: SeatType): string {
    switch (type) {
      case SeatType.Seat: return 'Standard'
      case SeatType.AccessibleSeat: return 'AccessibleSeat'
      case SeatType.Aisle: return 'Aisle'
      default: return 'Unknown'
    }
  }

  //creeate matrix

  openCreateForm(): void {
    this.openMatrixMenuId = null
    this.editingMatrixId = null
    this.isCreateFormOpen = true
    this.cdr.markForCheck()
  }

  closeCreateForm(): void {
    this.isCreateFormOpen = false
    this.cdr.markForCheck()
  }

  createMatrix(formValue: CreateLayoutMatrixDto): void {
    if (this.isCreatingMatrix) return

    this.isCreatingMatrix = true

    this.matrixService.createLayoutMatrix(formValue, this.auditoriumId).subscribe({
      next: createdMatrix => {
        this.isCreatingMatrix = false
        this.isCreateFormOpen = false

        this.loadMatrices()
        this.cdr.markForCheck()
      },
      error: err => {
        this.isCreatingMatrix = false
        console.error('Failed to create layout matrix', err)
        this.cdr.markForCheck()
      }
    });
  }

  //edit matrix

  openEditForm(matrix: LayoutMatrix): void {
    this.openMatrixMenuId = null
    this.isCreateFormOpen = false
    this.editingMatrixId = matrix.id
    this.cdr.markForCheck()
  }

  closeEditForm(): void {
    this.editingMatrixId = null
    this.openMatrixMenuId = null
    this.cdr.markForCheck()
  }

  isEditing(matrix: LayoutMatrix): boolean {
    return this.editingMatrixId === matrix.id
  }

  updateMatrix(matrixId: string, formValue: CreateLayoutMatrixDto): void {
    if (this.isSavingMatrix) return

    this.isSavingMatrix = true

    this.matrixService.updateLayoutMatrix(matrixId, {
      name: formValue.name,
      rows: formValue.rows,
      columns: formValue.columns
    }).subscribe({
      next: updatedMatrix => {
        this.isSavingMatrix = false
        this.editingMatrixId = null

        this.loadMatrices()
        this.selectMatrix(updatedMatrix)
        this.cdr.markForCheck()
      },
      error: err => {
        this.isSavingMatrix = false
        console.error('Failed to update layout matrix', err)
        this.cdr.markForCheck()
      }
    })
  }

  //delete matrix

  deleteMatrix(matrix: LayoutMatrix): void {
    if (this.isDeletingMatrix) return

    const confirmed = window.confirm(`Biztosan törölni szeretnéd ezt a layout matrixot: ${matrix.name}?`)
    if (!confirmed) return

    this.isDeletingMatrix = true

    this.matrixService.deleteLayoutMatrix(matrix.id).subscribe({
      next: () => {
        this.isDeletingMatrix = false
        this.editingMatrixId = null

        const deletedSelected = this.selectedMatrix?.id === matrix.id

        this.loadMatrices()

        if (deletedSelected) {
          this.selectedCellKey = null
          this.gridCells = []
          this.gridRows = 0
          this.gridColumns = 0
          this.seatService.clearSeatMap()
        }

        this.cdr.markForCheck()
      },
      error: err => {
        this.isDeletingMatrix = false
        console.error('Failed to delete layout matrix', err)
        this.cdr.markForCheck()
      }
    })
  }

  toggleMatrixMenu(matrixId: string): void {
    this.openMatrixMenuId = this.openMatrixMenuId === matrixId ? null : matrixId
    this.cdr.markForCheck()
  }

  closeMatrixMenu(): void {
    this.openMatrixMenuId = null
    this.cdr.markForCheck()
  }

  isMatrixMenuOpen(matrix: LayoutMatrix): boolean {
    return this.openMatrixMenuId === matrix.id
  }

  startEditMatrix(matrix: LayoutMatrix): void {
    this.openMatrixMenuId = null
    this.openEditForm(matrix)
  }

  startDeleteMatrix(matrix: LayoutMatrix): void {
    this.openMatrixMenuId = null
    this.deleteMatrix(matrix)
  }

  @HostListener('document:click', ['$event'])

  onDocumentClick(event: MouseEvent): void {
    if (!this.openMatrixMenuId) return

    const target = event.target as HTMLElement | null
    if (!target) return

    const clickedInsideMenu = !!target.closest('.matrix-item-menu')

    if (!clickedInsideMenu) {
      this.openMatrixMenuId = null
      this.cdr.markForCheck()
    }
  }

  saveSeat(formValue: UpdateSeatDto): void {
    const cell = this.selectedCell
    if (!cell?.seatId || this.isSavingSeat) return

    const dto: UpdateSeatDto = {
      seatLabel: formValue.seatLabel,
      sectorId: formValue.sectorId,
      priceOverride: formValue.priceOverride,
      seatType: formValue.seatType
    }

    this.isSavingSeat = true

    this.seatService.updateSeat(cell.seatId, dto).subscribe({
      next: updatedSeat => {
        this.isSavingSeat = false

        this.gridCells = this.gridCells.map(c =>
          c.seatId === updatedSeat.id
            ? {
              ...c,
              seatLabel: updatedSeat.seatLabel ?? null,
              seatType: updatedSeat.seatType,
              sectorId: updatedSeat.sectorId ?? null,
              priceOverride: updatedSeat.priceOverride ?? null
            }
            : c
        )

        this.cdr.markForCheck()
      },
      error: err => {
        this.isSavingSeat = false
        console.error('Failed to update seat', err)
        this.cdr.markForCheck()
      }
    })
  }

  applySectorToSelectedSeat(sectorId: string | null): void {
    const cell = this.selectedCell
    if (!cell?.seatId || this.isSavingSeat) return

    this.isSavingSeat = true

    const dto: UpdateSeatDto = {
      seatLabel: cell.seatLabel ?? null,
      sectorId,
      priceOverride: cell.priceOverride ?? null,
      seatType: cell.seatType
    }

    this.seatService.updateSeat(cell.seatId, dto).subscribe({
      next: updatedSeat => {
        this.isSavingSeat = false

        this.gridCells = this.gridCells.map(c =>
          c.seatId === updatedSeat.id
            ? {
              ...c,
              seatLabel: updatedSeat.seatLabel ?? null,
              seatType: updatedSeat.seatType,
              sectorId: updatedSeat.sectorId ?? null,
              priceOverride: updatedSeat.priceOverride ?? null
            }
            : c
        )

        this.cdr.markForCheck()
      },
      error: err => {
        this.isSavingSeat = false
        console.error('Failed to update seat sector', err)
        this.cdr.markForCheck()
      }
    })
  }

}
