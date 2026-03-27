import { Component, OnInit } from '@angular/core';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { Observable } from 'rxjs';
import { LayoutMatrix } from '../../models/layout-matrix';
import { array } from 'three/src/nodes/core/ArrayNode.js';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-layout-matrix-editor',
  standalone: false,
  templateUrl: './layout-matrix-editor.component.html',
  styleUrl: './layout-matrix-editor.component.sass'
})
export class LayoutMatrixEditorComponent implements OnInit {
  matrices$!: Observable<LayoutMatrix[]>
  selectedMatrix: LayoutMatrix | null = null

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
          this.selectedMatrix = null
          return
        }

        if (this.selectedMatrix) {
          const stillExists = matrices.find(m => m.id === this.selectedMatrix?.id)
          this.selectedMatrix = stillExists ?? matrices[0]
          return
        }

        this.selectedMatrix = matrices[0]
      },
      error: err => {
        console.error('Failed to load layout matrices', err)
        this.selectedMatrix = null
      }
    })
  }

  selectMatrix(matrix: LayoutMatrix): void {
    this.selectedMatrix = matrix
  }

  isSelected(matrix: LayoutMatrix): boolean {
    return this.selectedMatrix?.id === matrix.id
  }

  getGrid(): number[] {
    if (!this.selectedMatrix) return [];
    return Array.from({ length: this.selectedMatrix.rows * this.selectedMatrix.columns })
  }
}
