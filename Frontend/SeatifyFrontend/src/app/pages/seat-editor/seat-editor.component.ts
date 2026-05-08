import { Component, EventEmitter, input, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { MatrixCellVm } from '../../models/matrix-cell-vm';
import { SeatType, UpdateSeatDto } from '../../models/seat';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Sector } from '../../models/sector';

@Component({
  selector: 'app-seat-editor',
  standalone: false,
  templateUrl: './seat-editor.component.html',
  styleUrl: './seat-editor.component.sass'
})
export class SeatEditorComponent implements OnInit, OnChanges {
  @Input() cell: MatrixCellVm | null = null;
  @Input() isSaving = false;
  @Input() disabled = false;
  @Input() isBulkMode = false;
  @Input() currency = 'EUR';


  @Output() save = new EventEmitter<UpdateSeatDto>();

  form!: FormGroup;
  seatType = SeatType;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      seatLabel: ['', [Validators.maxLength(20)]],
      priceOverride: [null, [Validators.min(0), Validators.max(999999)]],
      seatType: [this.seatType.Seat, [Validators.required]]
    })

    this.applyCell()
    this.syncFormDisabledState()
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.form) return;

    if (changes['cell']) {
      this.applyCell();
    }

    if (changes['disabled'] || changes['isSaving']) {
      this.syncFormDisabledState()
    }
  }

  private applyCell(): void {
    const cell = this.cell;

    if (!cell) {
      if (this.isBulkMode) {
        this.form.reset({
          seatLabel: '',
          priceOverride: null,
          seatType: SeatType.Seat
        }, { emitEvent: false });
      } else {
        this.form.reset({
          seatLabel: '',
          priceOverride: null,
          seatType: SeatType.Seat
        }, { emitEvent: false });
      }

      return;
    }

    this.form.patchValue({
      seatLabel: cell.seatLabel ?? '',
      priceOverride: cell.priceOverride ?? null,
      seatType: cell.seatType
    }, { emitEvent: false });
  }

  private syncFormDisabledState(): void {
    if (this.disabled || this.isSaving) {
      this.form.disable({ emitEvent: false })
    } else {
      this.form.enable({ emitEvent: false })
    }
  }

  submit(): void {
    if (!this.isBulkMode && !this.cell?.seatId) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    if (this.isBulkMode) {
      this.save.emit({
        seatLabel: null,
        sectorId: null, // Sector is handled by separate sector editor usually, but we include it in dto if needed
        priceOverride: raw.priceOverride === '' || raw.priceOverride === null ? null : Number(raw.priceOverride),
        seatType: raw.seatType
      });
    } else {
      this.save.emit({
        seatLabel: raw.seatLabel?.trim() || null,
        sectorId: this.cell?.sectorId ?? null,
        priceOverride: raw.priceOverride === '' || raw.priceOverride === null ? null : Number(raw.priceOverride),
        seatType: raw.seatType
      });
    }
  }

  setSeatType(type: SeatType): void {
    this.form.get('seatType')?.setValue(type);
  }

  get seatLabelControl() {
    return this.form.get('seatLabel');
  }

  get priceOverrideControl() {
    return this.form.get('priceOverride');
  }

  get seatTypeControl() {
    return this.form.get('seatType');
  }
}
