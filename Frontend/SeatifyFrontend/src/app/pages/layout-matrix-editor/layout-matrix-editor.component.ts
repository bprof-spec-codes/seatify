import { Component, OnInit } from '@angular/core';
import { LayoutMatrixService } from '../../services/layout-matrix.service';
import { Observable } from 'rxjs';
import { LayoutMatrix } from '../../models/layout-matrix';
import { array } from 'three/src/nodes/core/ArrayNode.js';

@Component({
  selector: 'app-layout-matrix-editor',
  standalone: false,
  templateUrl: './layout-matrix-editor.component.html',
  styleUrl: './layout-matrix-editor.component.sass'
})
export class LayoutMatrixEditorComponent implements OnInit {
  matrices$!: Observable<LayoutMatrix[]>
  selectedMatrix: LayoutMatrix | null = null

  auditoriumId: string = 'aud-id-01' // mock data

  constructor(private matrixService: LayoutMatrixService) { }

  ngOnInit(): void {

    this.matrices$ = this.matrixService.LayoutMatrix$

    this.matrixService.getLayoutMatrixByAuditoriumId(this.auditoriumId).subscribe()
  }

  selectMatrix(matrix: LayoutMatrix): void {
    this.selectedMatrix = matrix
  }

  getGrid(): number[] {
    if (!this.selectedMatrix) return []
    return Array.from({ length: this.selectedMatrix.rows * this.selectedMatrix.columns })
  }


}
