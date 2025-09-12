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

  public get control() {
    return this.controlContainer?.control?.get(this.controlName) as AbstractControl;
  }
}
