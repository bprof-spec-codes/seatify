import { ChangeDetectionStrategy, Component, EventEmitter, HostListener, Input, Output, SimpleChanges } from '@angular/core';
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
  @Input() sectors: Sector[] = [];
  @Input() selectedSectorId: string | null = null;
  @Input() disabled = false;

  @Input() isCreateFormOpen = false;
  @Input() isCreatingSector = false;

  @Input() editingSectorId: string | null = null;
  @Input() isUpdatingSector = false;

  @Input() canAssign = false;

  @Input() isDeletingSector = false;

  @Output() applySector = new EventEmitter<string | null>();

  @Output() createClicked = new EventEmitter<void>();
  @Output() createCancelled = new EventEmitter<void>();
  @Output() createSubmitted = new EventEmitter<CreateUpdateSectorDto>();

  @Output() editClicked = new EventEmitter<string>();
  @Output() editCancelled = new EventEmitter<void>();
  @Output() editSubmitted = new EventEmitter<{ id: string; dto: CreateUpdateSectorDto }>();

  @Output() deleteClicked = new EventEmitter<string>();

  pendingSectorId: string | null = null;
  editingSectorInitialValue: CreateUpdateSectorDto | null = null;

  openSectorMenuId: string | null = null;
  hoveredSectorId: string | null = null;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedSectorId'] && this.selectedSectorId !== undefined) {
      // Only sync from input if it's NOT a bulk selection switch (null) 
      // or if we really want to reset it.
      if (this.selectedSectorId !== null || !this.pendingSectorId) {
        this.pendingSectorId = this.selectedSectorId;
      }
    }

    if (changes['editingSectorId'] || changes['sectors']) {
      this.syncEditingSectorInitialValue();
    }

    if (this.editingSectorId) {
      this.openSectorMenuId = null;
    }
  }

  selectSector(sectorId: string | null): void {
    if (this.disabled) return;
    this.pendingSectorId = sectorId;
    this.openSectorMenuId = null;
  }

  applyToSelected(): void {
    if (this.disabled || !this.canAssign) return;
    this.applySector.emit(this.pendingSectorId);
  }

  openCreateForm(): void {
    if (this.disabled) return;
    this.openSectorMenuId = null;
    this.createClicked.emit();
  }

  cancelCreate(): void {
    this.createCancelled.emit();
  }

  submitCreate(dto: CreateUpdateSectorDto): void {
    this.createSubmitted.emit(dto);
  }

  openEditForm(sectorId: string): void {
    if (this.disabled) return;
    this.editClicked.emit(sectorId);
  }

  cancelEdit(): void {
    this.editCancelled.emit();
  }

  submitEdit(dto: CreateUpdateSectorDto): void {
    if (!this.editingSectorId) return;

    this.editSubmitted.emit({
      id: this.editingSectorId,
      dto
    });
  }

  requestDelete(sectorId: string): void {
    if (this.disabled || this.isDeletingSector || this.isCreatingSector || this.isUpdatingSector) return;
    this.deleteClicked.emit(sectorId);
  }

  startEditSector(sectorId: string): void {
    this.openSectorMenuId = null;
    this.openEditForm(sectorId);
  }

  startDeleteSector(sectorId: string): void {
    this.openSectorMenuId = null;
    this.requestDelete(sectorId);
  }

  toggleSectorMenu(sectorId: string, event: MouseEvent): void {
    event.stopPropagation();
    this.openSectorMenuId = this.openSectorMenuId === sectorId ? null : sectorId;
  }

  isSectorMenuOpen(sector: Sector): boolean {
    return this.openSectorMenuId === sector.id;
  }

  isEditing(sector: Sector): boolean {
    return this.editingSectorId === sector.id;
  }

  trackBySectorId(index: number, sector: Sector): string {
    return sector.id;
  }

  private syncEditingSectorInitialValue(): void {
    if (!this.editingSectorId) {
      this.editingSectorInitialValue = null;
      return;
    }

    const sector = this.sectors.find(s => s.id === this.editingSectorId);

    this.editingSectorInitialValue = sector
      ? {
        name: sector.name,
        color: sector.color,
        basePrice: sector.basePrice
      }
      : null;
  }

  getSectorHoverBackground(color: string | null | undefined): string {
    if (!color) return '#f8fafc';
    return `${color}22`;
  }

  getSectorHoverBorder(color: string | null | undefined): string {
    if (!color) return '#d1d5db';
    return `${color}`;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.openSectorMenuId) return;

    const target = event.target as HTMLElement | null;
    if (!target) return;

    const clickedInsideMenu = !!target.closest('.sector-item-menu');

    if (!clickedInsideMenu) {
      this.openSectorMenuId = null;
    }
  }
}
