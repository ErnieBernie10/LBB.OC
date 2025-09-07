import { FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

export const setServerErrors = (formGroup: FormGroup, then?: () => void) => (response: HttpErrorResponse) => {
  if (response.status === 400) {
    formGroup.markAllAsDirty();
    formGroup.markAllAsTouched();
    const errors = response.error.errors;
    for (const field in errors) {
      if (formGroup.contains(field.toLowerCase())) {
        // Angular expects control names in lowercase or same as form control
        const control = formGroup.get(field.toLowerCase());
        control?.setErrors({ server: errors[field].join(', ') });
      }
    }
    then?.();
  }
};
