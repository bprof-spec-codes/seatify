import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Venue } from '../../models/venue';

@Component({
  selector: 'app-confirm-message',
  standalone: false,
  templateUrl: './confirm-message.component.html',
  styleUrl: './confirm-message.component.sass'
})
export class ConfirmMessageComponent {
  @Input() item: any;
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  isOpen = true;

  onConfirm(): void {
    this.confirm.emit();
    this.isOpen = false;
  }

  onCancel(): void {
    this.cancel.emit();
    this.isOpen = false;
  }
}
