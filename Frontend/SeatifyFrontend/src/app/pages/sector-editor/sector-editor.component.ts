import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { CreateUpdateSectorDto, Sector } from '../../models/sector';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

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

  @Input() isCreateFormOpen = false
  @Input() isCreatingSector = false

  @Input() editingSectorId: string | null = null
  @Input() isUpdatingSector = false

  @Input() canAssign = false

  @Output() applySector = new EventEmitter<string | null>()

  @Output() createClicked = new EventEmitter<void>()
  @Output() createCancelled = new EventEmitter<void>()
  @Output() createSubmitted = new EventEmitter<CreateUpdateSectorDto>()

  @Output() editClicked = new EventEmitter<string>()
  @Output() editCancelled = new EventEmitter<void>()
  @Output() editSubmitted = new EventEmitter<{ id: string; dto: CreateUpdateSectorDto }>()

  @Input() isDeletingSector = false

  @Output() deleteClicked = new EventEmitter<string>()

  pendingSectorId: string | null = null
  editingSectorInitialValue: CreateUpdateSectorDto | null = null

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedSectorId']) {
      this.pendingSectorId = this.selectedSectorId ?? null
    }

    if (changes['editingSectorId'] || changes['sectors']) {
      this.syncEditingSectorInitialValue()
    }
  }

  requestDelete(sectorId: string): void {
    if (this.disabled || this.isDeletingSector || this.isCreatingSector || this.isUpdatingSector) return
    this.deleteClicked.emit(sectorId)
  }

  private syncEditingSectorInitialValue(): void {
    if (!this.editingSectorId) {
      this.editingSectorInitialValue = null
      return
    }

    const sector = this.sectors.find(s => s.id === this.editingSectorId)

    this.editingSectorInitialValue = sector
      ? {
        name: sector.name,
        color: sector.color,
        basePrice: sector.basePrice
      }
      : null
  }

  selectSector(sectorId: string | null): void {
    if (this.disabled) return
    this.pendingSectorId = sectorId
  }

  applyToSelected(): void {
    if (this.disabled) return
    this.applySector.emit(this.pendingSectorId)
  }

  openCreateForm(): void {
    if (this.disabled) return
    this.createClicked.emit()
  }

  cancelCreate(): void {
    this.createCancelled.emit()
  }

  submitCreate(dto: CreateUpdateSectorDto): void {
    this.createSubmitted.emit(dto)
  }

  openEditForm(sectorId: string): void {
    if (this.disabled) return
    this.editClicked.emit(sectorId)
  }

  cancelEdit(): void {
    this.editCancelled.emit()
  }

  submitEdit(dto: CreateUpdateSectorDto): void {
    if (!this.editingSectorId) return

    this.editSubmitted.emit({
      id: this.editingSectorId,
      dto
    })
  }

  isEditing(sector: Sector): boolean {
    return this.editingSectorId === sector.id
  }

  trackBySectorId(index: number, sector: Sector): string {
    return sector.id
  }
}
