import { Component, EventEmitter, Input, OnChanges, OnInit, Output, output, SimpleChanges } from '@angular/core';
import { CreateLayoutMatrixDto } from '../../models/layout-matrix';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-layout-matrix-form',
  standalone: false,
  templateUrl: './layout-matrix-form.component.html',
  styleUrl: './layout-matrix-form.component.sass'
})
export class LayoutMatrixFormComponent implements OnInit, OnChanges {
  @Input() initialValue: CreateLayoutMatrixDto | null = null
  @Input() submitLabel = 'Save'
  @Input() isSubmitting = false

  @Output() submitted = new EventEmitter<CreateLayoutMatrixDto>()
  @Output() cancelled = new EventEmitter<void>()

  form!: FormGroup

  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      rows: [1, [Validators.required, Validators.min(1), Validators.max(100)]],
      columns: [1, [Validators.required, Validators.min(1), Validators.max(100)]],
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
      rows: 1,
      columns: 1
    }

    this.form.patchValue({
      name: value.name,
      rows: value.rows,
      columns: value.columns
    })
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched()
      return
    }

    const rawValue = this.form.getRawValue()

    this.submitted.emit({
      name: rawValue.name.trim(),
      rows: Number(rawValue.rows),
      columns: Number(rawValue.columns)
    })
  }

  cancel(): void {
    this.cancelled.emit()
  }

  get nameControl() {
    return this.form.get('name')
  }

  get rowsControl() {
    return this.form.get('rows')
  }

  get columnsControl() {
    return this.form.get('columns')
  }
}
