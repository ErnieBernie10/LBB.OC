import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

export type AlertType = 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'info';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert.html',
  styleUrl: './alert.scss',
})
export class Alert {
  @Input({
    transform: (value: string): AlertType => value as AlertType,
  })
  type: AlertType = 'primary';
  @Input() message: string = '';
  @Input() dismissible: boolean = false;
  @Output() dismissed = new EventEmitter<void>();

  visible: boolean = true;

  dismiss() {
    this.visible = false;
    this.dismissed.emit();
  }
}
