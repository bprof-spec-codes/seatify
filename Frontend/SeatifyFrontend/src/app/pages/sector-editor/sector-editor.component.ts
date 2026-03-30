import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { Sector } from '../../models/sector';

@Component({
  selector: 'app-sector-editor',
  standalone: false,
  templateUrl: './sector-editor.component.html',
  styleUrl: './sector-editor.component.sass',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SectorEditorComponent {
  @Input() sectors: Sector[] = []
  @Input() selectedSectorId: string | null = null
  @Input() disabled = false

  @Output() applySector = new EventEmitter<string | null>()

  pendingSectorId: string | null = null

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedSectorId']) {
      this.pendingSectorId = this.selectedSectorId ?? null
    }
  }

  selectSector(sectorId: string | null): void {
    if (this.disabled) return
    this.pendingSectorId = sectorId
  }

  applyToSelected(): void {
    if (this.disabled) return
    this.applySector.emit(this.pendingSectorId)
  }

  trackBySectorId(index: number, sector: Sector): string {
    return sector.id
  }
}
