import { Component, inject, Input, OnChanges, SimpleChanges, Injectable } from '@angular/core';
import { AbstractControl, ControlContainer } from '@angular/forms';

@Injectable()
export class FormLabelRegistry {
  private labels = new Map<string, string>();
  set(name: string, label: string): void {
    if (!name) return;
    this.labels.set(name, label);
  }
  get(name: unknown): string {
    if (typeof name !== 'string') return String(name ?? '');
    return this.labels.get(name) ?? name;
  }
}

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
export class FormInput implements OnChanges {
  private controlContainer: ControlContainer = inject(ControlContainer);
  private labelRegistry = inject(FormLabelRegistry, { optional: true });

  @Input() for: string = '';
  @Input() label: string = '';
  @Input() controlName: string = '';
  @Input() customMessages: { [key: string]: string | ((value: any) => string) } = {};
  @Input() required: boolean = false;

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['controlName'] || changes['label']) && this.labelRegistry) {
      this.labelRegistry.set(this.controlName, this.label);
    }
  }

  private defaultMessages: { [key: string]: string | ((value: any) => string) } = {
    required: 'This field is required',
    maxlength: (value) => `Maximum length is ${value.requiredLength} characters`,
    minlength: (value) => `Minimum length is ${value.requiredLength} characters`,
    email: 'Please enter a valid email address',
    min: (value) => `Minimum value is ${value.min}`,
    max: (value) => `Maximum value is ${value.max}`,
    NotEmptyError: $localize`This field is required`,
    GreaterThanError: (value) =>
      $localize`:validation|Numeric greater-than@@GreaterThanError:${this.label} must be greater than ${value.min}`,
    GreaterThanDateError: (value) =>
      $localize`:validation|Date greater-than@@GreaterThanDateError:${this.label} must be greater than ${this.lookupLabel(
        value?.property2Name
      )}`,
    LessThanDateError: (value) =>
      $localize`:validation|Date less-than@@LessThanDateError:${this.label} must be less than ${this.lookupLabel(
        value?.property2Name
      )}`,
    LengthExceededError: (value) => $localize`Maximum length is ${value.max}`,
  };

  private lookupLabel(otherName: unknown): string {
    // Falls back to the raw name if no registry is provided.
    return this.labelRegistry?.get(otherName) ?? String(otherName ?? '');
  }

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
    if (typeof value === 'string') return value;
    return `Invalid ${key}`;
  }
}
