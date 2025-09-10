import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface AlertItem {
  id?: number;
  title?: string;
  message: string;
  type: 'success' | 'danger' | 'warning' | 'info';
  timeoutMs?: number;
}

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  private _alerts: BehaviorSubject<AlertItem[]> = new BehaviorSubject<AlertItem[]>([]);
  public alerts = this._alerts.asObservable();
  private _idSeq = 1;

  public show(message: AlertItem) {
    const timeout = message.timeoutMs ?? 5000;
    const item: AlertItem = {
      id: this._idSeq++,
      message: message.message,
      title: message.title,
      type: message.type,
      timeoutMs: timeout,
    };
    const list = [...this._alerts.value, item];
    this._alerts.next(list);
    if (timeout > 0) {
      const id = item.id!;
      setTimeout(() => this.removeById(id), timeout);
    }
  }

  public removeAt(index: number) {
    const list = [...this._alerts.value];
    if (index >= 0 && index < list.length) {
      list.splice(index, 1);
      this._alerts.next(list);
    }
  }

  public removeById(id: number) {
    const list = this._alerts.value.filter((a) => a.id !== id);
    this._alerts.next(list);
  }
}
