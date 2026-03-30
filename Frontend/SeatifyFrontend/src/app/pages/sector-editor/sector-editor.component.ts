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

  @Output() applySector = new EventEmitter<string | null>()
  @Output() createClicked = new EventEmitter<void>()
  @Output() createCancelled = new EventEmitter<void>()
  @Output() createSubmitted = new EventEmitter<CreateUpdateSectorDto>()

  pendingSectorId: string | null = null
  createForm!: FormGroup

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.createForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      color: ['#FFFFFF', [Validators.required]],
      basePrice: [0, [Validators.required, Validators.min(0)]]
    })
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedSectorId']) {
      this.pendingSectorId = this.selectedSectorId ?? null
    }

    if (changes['isCreateFormOpen'] && this.createForm) {
      if (this.isCreateFormOpen) {
        this.resetCreateForm()
      }
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

  openCreateForm(): void {
    if (this.disabled) return
    this.createClicked.emit()
  }

  cancelCreate(): void {
    this.createCancelled.emit()
  }

  submitCreate(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched()
      return
    }

    const raw = this.createForm.getRawValue();

    this.createSubmitted.emit({
      name: raw.name.trim(),
      color: raw.color,
      basePrice: Number(raw.basePrice)
    })
  }

  private resetCreateForm(): void {
    this.createForm.reset(
      {
        name: '',
        color: '#FFFFFF',
        basePrice: 0
      },
      { emitEvent: false }
    )
  }

  trackBySectorId(index: number, sector: Sector): string {
    return sector.id
  }

  get nameControl() {
    return this.createForm.get('name')
  }

  get basePriceControl() {
    return this.createForm.get('basePrice')
  }
}
