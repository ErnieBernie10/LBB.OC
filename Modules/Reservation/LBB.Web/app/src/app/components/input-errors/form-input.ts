import { Component, inject, Input } from '@angular/core';
import { AbstractControl, ControlContainer } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  template: `
    <ng-container>
      <label [for]="controlName">
        {{ label }}
        @if (required) {
          <span class="required">*</span>
        }
      </label>
      <ng-content></ng-content>
      @if (shouldShowErrors) {
        @for (error of activeErrors; track error.key) {
          <small class="invalid-helper">{{ getErrorMessage(error.key, error.value) }}</small>
        }
      }
    </ng-container>
  `,
})
export class FormInput {
  private controlContainer: ControlContainer = inject(ControlContainer);
  @Input() for: string = '';
  @Input() label: string = '';
  @Input() controlName: string = '';
  @Input() customMessages: { [key: string]: string | ((value: any) => string) } = {};
  @Input() required: boolean = false;

  private defaultMessages: { [key: string]: string | ((value: any) => string) } = {
    required: 'This field is required',
    maxlength: (value) => `Maximum length is ${value.requiredLength} characters`,
    minlength: (value) => `Minimum length is ${value.requiredLength} characters`,
    email: 'Please enter a valid email address',
    min: (value) => `Minimum value is ${value.min}`,
    max: (value) => `Maximum value is ${value.max}`,
    NotEmptyValidator: $localize`This field is required`,
    GreaterThanValidator: (value) => $localize`Value must be greater than ${value.min}`,
    MaximumLengthValidator: (value) => $localize`Maximum length is ${value.max}`,
  };

  public get control() {
    return this.controlContainer?.control?.get(this.controlName) as AbstractControl;
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
    if (message) return message as string;
    // Fallback: if the control error's value is a string (e.g., server message), show it
    if (typeof value === 'string') return value;
    return `Invalid ${key}`;
  }
}
