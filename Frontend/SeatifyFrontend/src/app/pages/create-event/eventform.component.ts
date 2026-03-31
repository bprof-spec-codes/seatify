import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './eventform.component.html',
  //styleUrl: './create-event.component.sass'
})
export class EventFormComponent {
  @Input() initialData: any = null; // Update esetén ide jön az adat
  @Input() buttonText = 'Save';
  @Output() formSubmit = new EventEmitter<any>();

  eventform: FormGroup;

  constructor(private fb: FormBuilder) {
    this.eventform = this.fb.group({
      id: [0, Validators.required],
      name: ['', Validators.required],
      description: [''],
      logoImageUrl: [''],
      bannerImageUrl: [''],
      themePreset: ['Default (Blue)'],
      venueId: ['', Validators.required],
      auditoriumId: ['', Validators.required],
      startsAt: ['', Validators.required],
      endsAt: ['', Validators.required],
      basePrice: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit() {
    if (this.initialData) {
      this.eventform.patchValue(this.initialData);
    }
  }

  submit() {
    if (this.eventform.valid) {
      this.formSubmit.emit(this.eventform.value);
    } 
    else {
      // validation
      this.eventform.markAllAsTouched();
    }
  }

}
