import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { Observable } from 'rxjs';
import { LayoutMatrix } from '../../models/layout-matrix';
import { array } from 'three/src/nodes/core/ArrayNode.js';
import { ActivatedRoute } from '@angular/router';
import { MatrixCellVm } from '../../models/matrix-cell-vm';

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

  gridCells: MatrixCellVm[] = []
  selectedCellKey: string | null = null

  auditoriumId: string = ""

  constructor(private matrixService: LayoutMatrixService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.auditoriumId = this.route.snapshot.paramMap.get('auditoriumId') ?? 'aud-id-01';
    this.matrices$ = this.matrixService.LayoutMatrix$

    this.loadMatrices()
  }

  loadMatrices(): void {
    this.matrixService.getLayoutMatrixByAuditoriumId(this.auditoriumId).subscribe({
      next: matrices => {
        if (matrices.length === 0) {
          this.setSelectedMatrix(null)
          this.gridCells = []
          return
        }

        if (this.selectedMatrix) {
          const stillExists = matrices.find(m => m.id === this.selectedMatrix?.id)
          this.setSelectedMatrix(stillExists ?? matrices[0])
          return
        }

        this.setSelectedMatrix(matrices[0])
      },
      error: err => {
        console.error('Failed to load layout matrices', err)
        this.setSelectedMatrix(null)
        this.gridCells = []
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
    this.gridCells = matrix ? this.buildGridCells(matrix) : []
    this.selectedCellKey = null
  }

  private buildGridCells(matrix: LayoutMatrix): MatrixCellVm[] {
    const cells: MatrixCellVm[] = []

    for (let row = 1; row <= matrix.rows; row++) {
      for (let column = 1; column <= matrix.columns; column++) {
        cells.push({
          key: `${row}-${column}`,
          row,
          column
        })
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
  }

  isCellSelected(cell: MatrixCellVm): boolean {
    return this.selectedCellKey === cell.key
  }

  get selectedCell(): MatrixCellVm | null {
  if (!this.selectedCellKey) return null;
  return this.gridCells.find(c => c.key === this.selectedCellKey) ?? null;
}
}
