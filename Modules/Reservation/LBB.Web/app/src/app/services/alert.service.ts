import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

interface Alert {
  title: string;
  message: string;
  type: 'success' | 'danger' | 'warning' | 'info';
}

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  private _alerts: BehaviorSubject<Alert[]> = new BehaviorSubject<Alert[]>([]);
  public alerts = this._alerts.asObservable();

  public show(message: Alert) {
    this._alerts.next([...this._alerts.value, { message: message.message, title: message.title, type: message.type }]);
  }
}
