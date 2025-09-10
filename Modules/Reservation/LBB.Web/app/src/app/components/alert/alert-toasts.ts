import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AlertService } from '../../services/alert.service';
import { Alert as InlineAlert } from './alert';

@Component({
  selector: 'app-alert-toasts',
  standalone: true,
  imports: [AsyncPipe, InlineAlert],
  templateUrl: './alert-toasts.html',
  styleUrl: './alert-toasts.scss',
})
export class AlertToastsComponent {
  private svc = inject(AlertService);
  alerts$ = this.svc.alerts;
}
