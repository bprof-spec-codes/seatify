import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { MatrixCellVm } from '../../models/matrix-cell-vm';
import { SeatType, UpdateSeatDto } from '../../models/seat';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-seat-editor',
  standalone: false,
  templateUrl: './seat-editor.component.html',
  styleUrl: './seat-editor.component.sass'
})
export class SeatEditorComponent implements OnInit, OnChanges {
  @Input() cell: MatrixCellVm | null = null
  @Input() isSaving = false
  @Input() sectors: string[] = ["sector-id-01", "sector-id-02"]

  @Output() save = new EventEmitter<UpdateSeatDto>()

  form!: FormGroup
  seatType = SeatType

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      seatLabel: ['', [Validators.maxLength(20)]],
      sectorId: [null],
      priceOverride: [null, [Validators.min(0), Validators.max(999999)]],
      seatType: [this.seatType.Seat, [Validators.required]]
    })

    this.applyCell()
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.form) return

    if (changes['cell']) {
      this.applyCell()
    }
  }

  private applyCell(): void {
    const cell = this.cell

    if (!cell) {
      this.form.reset({
        seatLabel: '',
        sectorId: null,
        priceOverride: null,
        seatType: SeatType.Seat
      }, { emitEvent: false })

      return
    }

    this.form.patchValue({
      seatLabel: cell.seatLabel ?? '',
      sectorId: cell.sectorId ?? null,
      priceOverride: cell.priceOverride ?? null,
      seatType: cell.seatType
    }, { emitEvent: false })
  }

  submit(): void {
    if (!this.cell?.seatId) return

    if (this.form.invalid) {
      this.form.markAllAsTouched()
      return
    }

    const raw = this.form.getRawValue()

    this.save.emit({
      seatLabel: raw.seatLabel?.trim() || null,
      sectorId: raw.sectorId || null,
      priceOverride: raw.priceOverride === '' || raw.priceOverride === null ? null : Number(raw.priceOverride),
      seatType: raw.seatType
    })
  }

  get seatLabelControl() {
    return this.form.get('seatLabel')
  }

  get priceOverrideControl() {
    return this.form.get('priceOverride')
  }
}
