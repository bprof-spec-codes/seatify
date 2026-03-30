import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CreateUpdateSectorDto, Sector } from '../../models/sector';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-sector-form',
  standalone: false,
  templateUrl: './sector-form.component.html',
  styleUrl: './sector-form.component.sass'
})
export class SectorFormComponent implements OnInit, OnChanges {
  @Input() initialValue: CreateUpdateSectorDto | null = null
  @Input() submitLabel = 'Save'
  @Input() isSubmitting = false

  @Output() submitted = new EventEmitter<CreateUpdateSectorDto>()
  @Output() cancelled = new EventEmitter<void>()

  form!: FormGroup;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
      color: ['#FFFFFF', [Validators.required]],
      basePrice: [0, [Validators.required, Validators.min(0)]]
    })

    this.applyInitialValue()
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.form) return

    if (changes['initialValue']) {
      this.applyInitialValue()
    }
  }

  private applyInitialValue(): void {
    const value = this.initialValue ?? {
      name: '',
      color: '#FFFFFF',
      basePrice: 0
    }

    this.form.patchValue(
      {
        name: value.name,
        color: value.color,
        basePrice: value.basePrice
      },
      { emitEvent: false }
    )
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched()
      return
    }

    const raw = this.form.getRawValue()

    this.submitted.emit({
      name: raw.name.trim(),
      color: raw.color,
      basePrice: Number(raw.basePrice)
    })
  }

  cancel(): void {
    this.cancelled.emit()
  }

  get nameControl() {
    return this.form.get('name')
  }

  get basePriceControl() {
    return this.form.get('basePrice')
  }
}
