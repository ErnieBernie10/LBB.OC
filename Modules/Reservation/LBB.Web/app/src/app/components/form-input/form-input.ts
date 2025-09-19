import { Component, inject, Input } from '@angular/core';
import { AbstractControl, ControlContainer } from '@angular/forms';
import { Errors } from '../../models/errors';

type ErrorFactory = (label: string, value: any) => string;

const ORDERED_ERROR_KEYS = [
  Errors.NotEmptyValidator,
  Errors.NotEmptyError,
  Errors.InvalidEmailAddressError,
  Errors.InvalidPhoneNumberError,
  Errors.LengthExceededError,
  Errors.MaximumLengthValidator,
  Errors.LessThanOrEqualValidator,
  Errors.GreaterThanOrEqualValidator,
  Errors.GreaterThanError,
] as const;

const DEFAULT_MESSAGES: Record<string, ErrorFactory> = {
  NotEmptyValidator: (label) => $localize`:@@validation.required:${label} is required`,
  MaximumLengthValidator: (label, v) =>
    $localize`:@@validation.maxLength:${label} must be less than ${v?.maxLength} characters`,
  LessThanOrEqualValidator: (label, v) =>
    $localize`:@@validation.greaterThan:${label} must be greater than ${v?.greaterThanValue}`,
  GreaterThanOrEqualValidator: (label, v) =>
    $localize`:@@validation.greaterThan:${label} must be greater than ${v?.comparisonValue}`,
  NotEmptyError: (label) => $localize`:@@validation.required:${label} is required`,
  InvalidEmailAddressError: (label) => $localize`:@@validation.emailInvalid:${label} is not valid`,
  InvalidPhoneNumberError: (label) => $localize`:@@validation.phoneInvalid:${label} is not valid`,
  LengthExceededError: (label, v) =>
    $localize`:@@validation.maxLength:${label} must be less than ${v?.maxLength} characters`,
  GreaterThanError: (label, v) =>
    $localize`:@@validation.greaterThan:${label} must be greater than ${v?.greaterThanValue}`,
};
@Component({
  selector: 'app-input',
  standalone: true,
  template: `
    <ng-container>
      <label [for]="controlName">
        <ng-content select="[appInputLabel]"></ng-content>
        @if (!hasProjectedLabel) {
          <span>{{ label }}</span>
        }
        @if (required) {
          <span class="required">*</span>
        }
      </label>
      <ng-content></ng-content>

      @if (control?.touched && control?.invalid) {
        <small>{{ resolveErrorMessage() }}</small>
      }
    </ng-container>
  `,
})
export class FormInput {
  private controlContainer: ControlContainer = inject(ControlContainer);

  @Input() for: string = '';
  @Input() label: string = '';
  @Input() controlName: string = '';
  @Input() required: boolean = false;

  // Optional overrides for specific error codes
  @Input() customMessages: { [key: string]: string | ErrorFactory } = {};

  // If you project a node like <span appInputLabel i18n>…</span>, set this to true
  // from the parent if needed; Angular can't detect projection presence synchronously
  // without extra querying logic. Keeping a lightweight flag is simpler and explicit.
  @Input() hasProjectedLabel: boolean = false;

  public get control() {
    return this.controlContainer?.control?.get(this.controlName) as AbstractControl;
  }

  private firstError(): { key: string; value: any } | null {
    const errors = this.control?.errors ?? null;
    if (!errors) return null;

    // Show mapped errors in a deterministic priority
    for (const k of ORDERED_ERROR_KEYS) {
      const v = (errors as any)[k];
      if (v !== undefined) return { key: k as string, value: v };
    }

    // Otherwise show whichever comes first
    const [firstKey] = Object.keys(errors);
    if (firstKey) return { key: firstKey, value: (errors as any)[firstKey] };
    return null;
  }

  private getLabelForError(): string {
    // Use the input label for messages; ensure usage provides a localized static string via i18n-label
    return this.label || '';
  }

  private makeMessage(key: string, value: any): string {
    const label = this.getLabelForError();

    // 1) Caller override
    const custom = this.customMessages[key];
    if (custom) {
      return typeof custom === 'function' ? (custom as ErrorFactory)(label, value) : String(custom);
    }

    // 2) Built-in mapping
    const builtin = DEFAULT_MESSAGES[key];
    if (builtin) {
      return builtin(label, value);
    }

    // 4) Final generic fallback
    return $localize`:@@validation.genericInvalid:${label} is invalid`;
  }

  public resolveErrorMessage(): string {
    const err = this.firstError();
    if (!err) return '';
    return this.makeMessage(err.key, err.value);
  }
}
