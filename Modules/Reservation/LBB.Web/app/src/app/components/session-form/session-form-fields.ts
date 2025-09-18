import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FormInput } from '../form-input/form-input';
import { InvalidPipe } from '../../pipes/invalid-pipe';
import { Errors } from '../../models/errors';

@Component({
  selector: 'app-session-form-fields',
  standalone: true,
  imports: [ReactiveFormsModule, FormInput, InvalidPipe],
  templateUrl: './session-form-fields.html',
})
export class SessionFormFieldsComponent {
  @Input({ required: true }) form!: FormGroup;
  protected readonly Errors = Errors;

  public get title() {
    return this.form.get('title');
  }
  public get description() {
    return this.form.get('description');
  }
  public get location() {
    return this.form.get('location');
  }
  public get start() {
    return this.form.get('start');
  }
  public get end() {
    return this.form.get('end');
  }
  public get type() {
    return this.form.get('type');
  }
  public get capacity() {
    return this.form.get('capacity');
  }
}
