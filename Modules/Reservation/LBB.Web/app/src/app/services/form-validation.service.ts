import { inject, Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { normalizeFieldPath } from '../util/formutils';
import { AlertService } from './alert.service';
import { environment } from '../../environments/environment';
import { ApiException } from '../api/api';

@Injectable({
  providedIn: 'root',
})
export class FormValidationService {
  private alertService = inject(AlertService);
  public setServerErrors(formGroup: FormGroup, then?: () => void) {
    return (response: ApiException) => {
      const res = JSON.parse(response.response);
      if (response.status === 400) {
        formGroup.markAllAsTouched();

        // Prefer detailed errors if available; fallback to simple errors dictionary
        const detailed = res?.errorsDetailed as
          | Record<string, Array<{ message: string; errorCode: string; metadata?: any }>>
          | undefined;

        if (detailed === undefined || detailed === null) {
          this.alertService.show({
            title: 'Error',
            message: $localize`An unexpected error occurred, please contact your administrator.`,
            type: 'danger',
          });
        }

        if (detailed && typeof detailed === 'object') {
          for (const field in detailed) {
            const control = formGroup.get(normalizeFieldPath(field));
            const errCol: Record<string, any> = {};
            for (const err of detailed[field] ?? []) {
              // Use the server-provided validator code as the error key so the UI can map messages
              errCol[err.errorCode] = err;
            }
            // Merge with existing errors if any
            if (control) {
              const existing = control.errors ?? {};
              control.setErrors({ ...existing, ...errCol });
            }
          }
        }
        if (!environment.production) {
          console.log(detailed);
        }

        then?.();
      }
    };
  }
}
