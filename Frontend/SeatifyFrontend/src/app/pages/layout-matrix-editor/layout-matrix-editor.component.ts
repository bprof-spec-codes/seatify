import { ChangeDetectionStrategy, ChangeDetectorRef, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { BehaviorSubject, catchError, Observable, of, switchMap, tap } from 'rxjs';
import { CreateLayoutMatrixDto, LayoutMatrix } from '../../models/layout-matrix';
import { ActivatedRoute } from '@angular/router';
import { MatrixCellVm } from '../../models/matrix-cell-vm';
import { SeatService } from '../../services/seat.service';
import { SeatType, UpdateSeatDto } from '../../models/seat';
import { SeatMap } from '../../models/seat-map';
import { CreateUpdateSectorDto, Sector } from '../../models/sector';
import { SectorService } from '../../services/sector.service';
import { Location } from '@angular/common';
import { SeatOverrideService } from '../../services/seat-override.service';
import { EffectiveSeat, EffectiveSeatMap } from '../../models/seat-override';

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
  seatMap$!: Observable<SeatMap | EffectiveSeatMap | null>;

  gridCells: MatrixCellVm[] = []
  selectedCellKeys: string[] = []

  auditoriumId: string = ''
  seatType = SeatType

  /** Kontextus mód: auditorium (alap) | event | occurrence */
  editorContext: 'auditorium' | 'event' | 'occurrence' = 'auditorium'
  contextEventId: string | null = null
  contextOccurrenceId: string | null = null

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

  isSelecting = false
  selectionStartCell: MatrixCellVm | null = null

  @ViewChild('gridWrapper')
  gridWrapper?: ElementRef<HTMLDivElement>

  matrixTool: 'select' | 'pan' = 'select'
  matrixZoom = 1
  matrixCellSize = 36

  readonly minMatrixZoom = 0.45
  readonly maxMatrixZoom = 2
  readonly matrixZoomStep = 0.1

  isPanning = false
  private panStartX = 0
  private panStartY = 0
  private panStartScrollLeft = 0
  private panStartScrollTop = 0

  sectors$!: Observable<Sector[]>;
  isCreateSectorFormOpen = false
  isCreatingSector = false

  editingSectorId: string | null = null
  isUpdatingSector = false

  isDeletingSector = false
  sectorErrorMessage: string | null = null

  private latestSectors: Sector[] = []

  constructor(
    private matrixService: LayoutMatrixService,
    private route: ActivatedRoute,
    private seatService: SeatService,
    private cdr: ChangeDetectorRef,
    private sectorService: SectorService,
    private location: Location,
    private seatOverrideService: SeatOverrideService,
  ) { }

  ngOnInit(): void {
    this.auditoriumId = this.route.snapshot.paramMap.get('auditoriumId') ?? '';

    // Kontextus meghatározása query paraméterekből
    this.contextOccurrenceId = this.route.snapshot.queryParamMap.get('occurrenceId');
    this.contextEventId = this.route.snapshot.queryParamMap.get('eventId');

    if (this.contextOccurrenceId) {
      this.editorContext = 'occurrence';
    } else if (this.contextEventId) {
      this.editorContext = 'event';
    } else {
      this.editorContext = 'auditorium';
    }

    this.matrices$ = this.matrixService.LayoutMatrix$
    this.sectors$ = this.sectorService.sector$;

    this.sectorService.getSectorsByAuditoriumId(this.auditoriumId).subscribe()

    this.seatMap$ = this.selectedMatrixSubject.pipe(
      tap(() => {
        this.gridCells = []
        this.gridRows = 0
        this.gridColumns = 0
        this.selectedCellKeys = []
        this.cdr.markForCheck()
      }),
      switchMap((matrix): Observable<SeatMap | EffectiveSeatMap | null> => {
        if (!matrix) return of(null)
        return this.loadSeatMapForContext(matrix.id)
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

          setTimeout(() => {
            this.fitMatrixToScreen()
          })
        }
        this.cdr.markForCheck()
      })
    )

    this.sectors$ = this.sectorService.sector$.pipe(
      tap(sectors => {
        this.latestSectors = sectors
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
    this.selectedCellKeys = []
    this.cdr.markForCheck()
  }

  goBack(): void {
    this.location.back()
  }

  setMatrixTool(tool: 'select' | 'pan'): void {
    this.matrixTool = tool

    if (tool === 'pan') {
      this.isSelecting = false
      this.selectionStartCell = null
    }

    this.cdr.markForCheck()
  }

  zoomIn(): void {
    this.matrixZoom = Math.min(
      this.maxMatrixZoom,
      Number((this.matrixZoom + this.matrixZoomStep).toFixed(2))
    )

    this.cdr.markForCheck()
  }

  zoomOut(): void {
    this.matrixZoom = Math.max(
      this.minMatrixZoom,
      Number((this.matrixZoom - this.matrixZoomStep).toFixed(2))
    )

    this.cdr.markForCheck()
  }

  resetMatrixZoom(): void {
    this.matrixZoom = 1
    this.cdr.markForCheck()
  }

  fitMatrixToScreen(): void {
    if (!this.gridWrapper?.nativeElement || this.gridRows <= 0 || this.gridColumns <= 0) {
      return
    }

    const wrapper = this.gridWrapper.nativeElement
    const availableWidth = wrapper.clientWidth - 56
    const availableHeight = wrapper.clientHeight - 56
    const gap = 8

    const maxCellByWidth = Math.floor((availableWidth - ((this.gridColumns - 1) * gap)) / this.gridColumns)
    const maxCellByHeight = Math.floor((availableHeight - ((this.gridRows - 1) * gap)) / this.gridRows)
    const nextCellSize = Math.min(maxCellByWidth, maxCellByHeight, 38)

    this.matrixCellSize = Math.max(nextCellSize, 14)
    this.matrixZoom = 1

    this.cdr.markForCheck()
  }

  selectAllSeatsInMatrix(): void {
    this.selectedCellKeys = this.gridCells
      .filter(cell => cell.seatType !== SeatType.Aisle)
      .map(cell => cell.key)

    this.updateSeatEditModel()
    this.cdr.markForCheck()
  }

  clearGridSelection(): void {
    this.selectedCellKeys = []
    this.updateSeatEditModel()
    this.cdr.markForCheck()
  }

  get matrixZoomPercent(): number {
    return Math.round(this.matrixZoom * 100)
  }

  get matrixGridStyles(): Record<string, string> {
    return {
      '--matrix-columns': String(this.gridColumns || 1),
      '--matrix-cell-size': `${this.matrixCellSize}px`,
      '--matrix-zoom': String(this.matrixZoom)
    }
  }

  onGridWrapperMouseDown(event: MouseEvent): void {
    if (this.matrixTool !== 'pan') return
    if (event.button !== 0) return
    if (!this.gridWrapper?.nativeElement) return

    event.preventDefault()

    const wrapper = this.gridWrapper.nativeElement

    this.isPanning = true
    this.panStartX = event.clientX
    this.panStartY = event.clientY
    this.panStartScrollLeft = wrapper.scrollLeft
    this.panStartScrollTop = wrapper.scrollTop

    this.cdr.markForCheck()
  }

  @HostListener('document:mousemove', ['$event'])
  onDocumentMouseMove(event: MouseEvent): void {
    if (!this.isPanning) return
    if (!this.gridWrapper?.nativeElement) return

    const wrapper = this.gridWrapper.nativeElement
    const deltaX = event.clientX - this.panStartX
    const deltaY = event.clientY - this.panStartY

    wrapper.scrollLeft = this.panStartScrollLeft - deltaX
    wrapper.scrollTop = this.panStartScrollTop - deltaY
  }

  onGridWheel(event: WheelEvent): void {
    if (!event.ctrlKey && !event.metaKey) return

    event.preventDefault()

    if (event.deltaY > 0) {
      this.zoomOut()
    } else {
      this.zoomIn()
    }
  }

  /**
   * Kontextus alapján tölti be a helyes seat map-et:
   * - occurrence: merged (occurrence ?? event ?? auditorium)
   * - event: merged (event ?? auditorium)
   * - auditorium: alap
   */
  private loadSeatMapForContext(matrixId: string): Observable<SeatMap | EffectiveSeatMap | null> {
    if (this.editorContext === 'occurrence' && this.contextOccurrenceId) {
      return this.seatOverrideService
        .getEffectiveSeatMapForOccurrence(this.contextOccurrenceId, matrixId)
        .pipe(catchError(err => {
          console.error('Failed to load occurrence seat map', err)
          return of(null)
        }))
    }
    if (this.editorContext === 'event' && this.contextEventId) {
      return this.seatOverrideService
        .getEffectiveSeatMapForEvent(this.contextEventId, matrixId)
        .pipe(catchError(err => {
          console.error('Failed to load event seat map', err)
          return of(null)
        }))
    }
    // Auditorium (alap) mód
    return this.seatService.getSeatMapByMatrixId(matrixId).pipe(
      catchError(err => {
        console.error('Failed to load seat map', err)
        return of(null)
      })
    )
  }

  private buildGridCellsFromSeatMap(seatMap: SeatMap | EffectiveSeatMap): MatrixCellVm[] {
    const cells: MatrixCellVm[] = [];

    // EffectiveSeatMap (override mód) vagy SeatMap (alap mód) ?
    const isEffective = 'context' in seatMap;

    if (isEffective) {
      const effective = seatMap as EffectiveSeatMap;
      const seatLookup = new Map<string, EffectiveSeat>()
      for (const s of effective.seats) {
        seatLookup.set(`${s.row}-${s.column}`, s)
      }
      for (let row = 1; row <= effective.rows; row++) {
        for (let col = 1; col <= effective.columns; col++) {
          const key = `${row}-${col}`
          const seat = seatLookup.get(key)
          cells.push({
            key, row, column: col,
            seatId: seat?.seatId ?? null,
            seatLabel: seat?.seatLabel ?? null,
            seatType: (seat?.seatType as unknown as SeatType) ?? SeatType.Seat,
            sectorId: seat?.sectorId ?? null,
            priceOverride: seat?.priceOverride ?? null,
            sectorSource: seat?.sectorSource ?? 'auditorium',
            priceSource: seat?.priceSource ?? 'auditorium',
            seatTypeSource: seat?.seatTypeSource ?? 'auditorium'
          })
        }
      }
    } else {
      const basic = seatMap as SeatMap;
      const seatLookup = new Map<string, typeof basic.seats[number]>()
      for (const seat of basic.seats) {
        seatLookup.set(`${seat.row}-${seat.column}`, seat)
      }
      for (let row = 1; row <= basic.rows; row++) {
        for (let col = 1; col <= basic.columns; col++) {
          const key = `${row}-${col}`
          const seat = seatLookup.get(key)
          cells.push({
            key, row, column: col,
            seatId: seat?.id ?? null,
            seatLabel: seat?.seatLabel ?? null,
            seatType: seat?.seatType ?? SeatType.Seat,
            sectorId: seat?.sectorId ?? null,
            priceOverride: seat?.priceOverride ?? null,
            sectorSource: 'auditorium',
            priceSource: 'auditorium',
            seatTypeSource: 'auditorium'
          })
        }
      }
    }

    return cells
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

  selectCell(cell: MatrixCellVm, event?: MouseEvent): void {
    if (event?.shiftKey && this.selectedCellKeys.length > 0) {
      this.selectRange(cell);
      return;
    }

    const isAlreadySelected = this.selectedCellKeys.includes(cell.key)

    if (event?.ctrlKey || event?.metaKey) {
      if (isAlreadySelected) {
        this.selectedCellKeys = this.selectedCellKeys.filter(key => key !== cell.key)
      } else {
        this.selectedCellKeys = [...this.selectedCellKeys, cell.key]
      }
    } else {
      if (isAlreadySelected && this.selectedCellKeys.length === 1) {
        this.selectedCellKeys = []
      } else {
        this.selectedCellKeys = [cell.key]
      }
    }

    this.updateSeatEditModel()
    this.cdr.markForCheck()
  }

  private selectRange(targetCell: MatrixCellVm): void {
    if (this.selectedCellKeys.length === 0) return;

    const firstKey = this.selectedCellKeys[0];
    const firstCell = this.gridCells.find(c => c.key === firstKey);

    if (!firstCell) return;

    const minRow = Math.min(firstCell.row, targetCell.row);
    const maxRow = Math.max(firstCell.row, targetCell.row);
    const minCol = Math.min(firstCell.column, targetCell.column);
    const maxCol = Math.max(firstCell.column, targetCell.column);

    const rangeKeys = this.gridCells
      .filter(c => c.row >= minRow && c.row <= maxRow && c.column >= minCol && c.column <= maxCol)
      .map(c => c.key);

    this.selectedCellKeys = Array.from(new Set([...this.selectedCellKeys, ...rangeKeys]));
    this.updateSeatEditModel();
  }

  private updateSeatEditModel(): void {
    if (this.selectedCells.length === 1) {
      const selected = this.selectedCells[0]

      this.seatEditModel = {
        seatLabel: selected.seatLabel ?? '',
        sectorId: selected.sectorId ?? null,
        priceOverride: selected.priceOverride ?? null,
        seatType: selected.seatType
      }
    } else {
      this.seatEditModel = null
    }
  }

  onMouseDown(cell: MatrixCellVm, event: MouseEvent): void {
    if (this.matrixTool !== 'select') return
    if (event.button !== 0) return

    event.preventDefault()
    event.stopPropagation()

    if (!event.ctrlKey && !event.metaKey && !event.shiftKey) {
      this.isSelecting = true
      this.selectionStartCell = cell
      this.selectedCellKeys = [cell.key]
    } else if (event.shiftKey) {
      this.selectRange(cell)
    } else {
      this.selectCell(cell, event)
    }

    this.updateSeatEditModel()
    this.cdr.markForCheck()
  }

  onMouseEnter(cell: MatrixCellVm): void {
    if (this.matrixTool !== 'select') return
    if (!this.isSelecting || !this.selectionStartCell) return

    const start = this.selectionStartCell
    const minRow = Math.min(start.row, cell.row)
    const maxRow = Math.max(start.row, cell.row)
    const minCol = Math.min(start.column, cell.column)
    const maxCol = Math.max(start.column, cell.column)

    this.selectedCellKeys = this.gridCells
      .filter(c => c.row >= minRow && c.row <= maxRow && c.column >= minCol && c.column <= maxCol)
      .map(c => c.key)

    this.updateSeatEditModel()
    this.cdr.markForCheck()
  }

  @HostListener('document:mouseup')
  onMouseUp(): void {
    this.isSelecting = false
    this.selectionStartCell = null
    this.isPanning = false
    this.cdr.markForCheck()
  }

  isCellSelected(cell: MatrixCellVm): boolean {
    return this.selectedCellKeys.includes(cell.key)
  }

  get selectedCells(): MatrixCellVm[] {
    if (this.selectedCellKeys.length === 0) return []
    return this.gridCells.filter(c => this.selectedCellKeys.includes(c.key))
  }

  get selectedCell(): MatrixCellVm | null {
    return this.selectedCells.length === 1 ? this.selectedCells[0] : null
  }

  get hasSelection(): boolean {
    return this.selectedCellKeys.length > 0
  }

  get isBulkSelection(): boolean {
    return this.selectedCellKeys.length > 1
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
          this.selectedCellKeys = []
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

  //update seat

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

  applySectorToSelectedSeats(sectorId: string | null): void {
    if (this.selectedCellKeys.length === 0 || this.isSavingSeat) return

    if (this.selectedCellKeys.length === 1 && this.editorContext === 'auditorium') {
      this.applySectorToSingleSeat(sectorId)
      return
    }

    const seatIds = this.selectedCells.map(c => c.seatId).filter((id): id is string => !!id)
    if (seatIds.length === 0) return

    this.isSavingSeat = true
    const overrideSource = this.editorContext

    // Kontextus-specifikus endpoint
    const save$ = this.editorContext === 'occurrence' && this.contextOccurrenceId
      ? this.seatOverrideService.bulkUpsertOccurrenceOverride(this.contextOccurrenceId, {
          seatIds, sectorId: sectorId ?? undefined, clearSector: sectorId === null
        })
      : this.editorContext === 'event' && this.contextEventId
        ? this.seatOverrideService.bulkUpsertEventOverride(this.contextEventId, {
            seatIds, sectorId: sectorId ?? undefined, clearSector: sectorId === null
          })
        : this.seatService.bulkUpdateSeats({
            seatIds, sectorId: sectorId ?? undefined, clearSector: sectorId === null
          })

    save$.subscribe({
      next: () => {
        this.isSavingSeat = false
        this.updateLocalCells({ sectorId, sectorSource: overrideSource })
        this.cdr.markForCheck()
      },
      error: err => {
        this.isSavingSeat = false
        console.error('Failed to bulk update sectors', err)
        this.cdr.markForCheck()
      }
    })
  }

  private applySectorToSingleSeat(sectorId: string | null): void {
    const cell = this.selectedCell
    if (!cell?.seatId) return

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
        this.updateLocalCells({
          seatId: updatedSeat.id,
          seatLabel: updatedSeat.seatLabel ?? null,
          seatType: updatedSeat.seatType,
          sectorId: updatedSeat.sectorId ?? null,
          priceOverride: updatedSeat.priceOverride ?? null
        })
        this.cdr.markForCheck()
      },
      error: err => {
        this.isSavingSeat = false
        console.error('Failed to update seat sector', err)
        this.cdr.markForCheck()
      }
    })
  }

  bulkUpdateSeats(formValue: Partial<UpdateSeatDto>): void {
    if (this.selectedCellKeys.length <= 1 || this.isSavingSeat) return

    const seatIds = this.selectedCells.map(c => c.seatId).filter((id): id is string => !!id)
    if (seatIds.length === 0) return

    this.isSavingSeat = true

    this.seatService.bulkUpdateSeats({
      seatIds,
      seatType: formValue.seatType,
      priceOverride: formValue.priceOverride,
      clearPriceOverride: formValue.priceOverride === null
    }).subscribe({
      next: () => {
        this.isSavingSeat = false
        this.updateLocalCells({
          seatType: formValue.seatType,
          priceOverride: formValue.priceOverride
        })
        this.cdr.markForCheck()
      },
      error: err => {
        this.isSavingSeat = false
        console.error('Failed to bulk update seats', err)
        this.cdr.markForCheck()
      }
    })
  }

  private updateLocalCells(patch: Partial<MatrixCellVm>): void {
    const selectedKeys = this.selectedCellKeys

    this.gridCells = this.gridCells.map(cell => {
      if (patch.seatId) {
        // Single update by ID
        if (cell.seatId === patch.seatId) {
          return { ...cell, ...patch }
        }
      } else if (selectedKeys.includes(cell.key)) {
        // Bulk update by selection
        return { ...cell, ...patch }
      }
      return cell
    })
  }


  //sector

  openCreateSectorForm(): void {
    this.isCreateSectorFormOpen = true
    this.cdr.markForCheck()
  }

  closeCreateSectorForm(): void {
    this.isCreateSectorFormOpen = false
    this.cdr.markForCheck()
  }

  createSector(dto: CreateUpdateSectorDto): void {
    if (!this.auditoriumId || this.isCreatingSector) return

    this.isCreatingSector = true

    this.sectorService.createSector(this.auditoriumId, dto).subscribe({
      next: () => {
        this.isCreatingSector = false
        this.isCreateSectorFormOpen = false
        this.cdr.markForCheck()
      },
      error: err => {
        this.isCreatingSector = false
        console.error('Failed to create sector', err)
        this.cdr.markForCheck()
      }
    })
  }

  openEditSectorForm(sectorId: string): void {
    this.isCreateSectorFormOpen = false
    this.editingSectorId = sectorId
    this.cdr.markForCheck()
  }

  closeEditSectorForm(): void {
    this.editingSectorId = null
    this.cdr.markForCheck()
  }
  updateSector(event: { id: string; dto: CreateUpdateSectorDto }): void {
    if (this.isUpdatingSector) return

    this.isUpdatingSector = true

    this.sectorService.updateSector(event.id, event.dto).subscribe({
      next: () => {
        this.isUpdatingSector = false
        this.editingSectorId = null
        this.cdr.markForCheck()
      },
      error: err => {
        this.isUpdatingSector = false
        console.error('Failed to update sector', err)
        this.cdr.markForCheck()
      }
    })
  }

  deleteSector(sectorId: string): void {
    if (this.isDeletingSector) return

    const sector = this.findSectorById(sectorId)
    const sectorName = sector?.name ?? 'this sector'

    const confirmed = window.confirm(`Biztosan törölni szeretnéd ezt a szektort: ${sectorName}?`)
    if (!confirmed) return

    this.isDeletingSector = true
    this.sectorErrorMessage = null

    this.sectorService.deleteSector(sectorId).subscribe({
      next: () => {
        this.isDeletingSector = false

        this.gridCells = this.gridCells.map(cell =>
          cell.sectorId === sectorId
            ? { ...cell, sectorId: null }
            : cell
        )

        if (this.editingSectorId === sectorId) {
          this.editingSectorId = null
        }

        this.cdr.markForCheck()
      },
      error: err => {
        this.isDeletingSector = false

        const backendMessage =
          err?.error?.message ||
          err?.error?.title ||
          err?.error ||
          null

        console.error('Failed to delete sector', err)

        if (typeof backendMessage === 'string' && backendMessage.trim()) {
          alert(backendMessage)
        } else {
          alert('You cannot delete this sector because there are seats assigned to it. Please reassign or delete those seats before deleting the sector.')
        }

        this.cdr.markForCheck()
      }
    })
  }

  private findSectorById(sectorId: string): Sector | undefined {
    return this.latestSectors.find(s => s.id === sectorId)
  }

  getSectorColor(sectorId: string | null | undefined): string | null {
    if (!sectorId) return null

    const sector = this.latestSectors.find(s => s.id === sectorId)
    return sector?.color ?? null
  }

}
