import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type AlertType = 'primary' | 'secondary' | 'success' | 'warning' | 'danger';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert.html',
  styleUrl: './alert.scss',
})
export class Alert {
  @Input({
    transform: (value: string): 'primary' | 'secondary' | 'success' | 'warning' | 'danger' => value as AlertType,
  })
  type: AlertType = 'primary';
  @Input() message: string = '';
  @Input() dismissible: boolean = false;

  visible: boolean = true;

  dismiss() {
    this.visible = false;
  }
}
