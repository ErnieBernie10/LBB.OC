import { Component, forwardRef, inject, Input } from '@angular/core';
import { AbstractControl, ControlContainer, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  template: `
    <ng-container>
      <label [for]="formControlName">{{ label }}</label>
      <ng-content></ng-content>
      @if (shouldShowErrors) {
        @for (error of activeErrors; track error.key) {
          <small class="invalid-helper">{{ getErrorMessage(error.key, error.value) }}</small>
        }
      }
    </ng-container>
  `,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormInput),
      multi: true,
    },
  ],
})
export class FormInput implements ControlValueAccessor {
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
  setDisabledState?(isDisabled: boolean): void {}

  private controlContainer: ControlContainer = inject(ControlContainer);
  @Input() for: string = '';
  @Input() label: string = '';
  @Input() formControlName: string = '';
  @Input() customMessages: { [key: string]: string | ((value: any) => string) } = {};

  private defaultMessages: { [key: string]: string | ((value: any) => string) } = {
    required: 'This field is required',
    maxlength: (value) => `Maximum length is ${value.requiredLength} characters`,
    minlength: (value) => `Minimum length is ${value.requiredLength} characters`,
    email: 'Please enter a valid email address',
    min: (value) => `Minimum value is ${value.min}`,
    max: (value) => `Maximum value is ${value.max}`,
  };

  public get control() {
    return this.controlContainer?.control?.get(this.formControlName) as AbstractControl;
  }

  get shouldShowErrors(): boolean {
    return !!this.control && this.control.touched && this.control.dirty && this.control.errors !== null;
  }

  get activeErrors(): Array<{ key: string; value: any }> {
    if (!this.control?.errors) return [];
    return Object.entries(this.control.errors).map(([key, value]) => ({ key, value }));
  }

  getErrorMessage(key: string, value: any): string {
    const message = this.customMessages[key] || this.defaultMessages[key];
    if (typeof message === 'function') {
      return message(value);
    }
    return message || `Invalid ${key}`;
  }
}
