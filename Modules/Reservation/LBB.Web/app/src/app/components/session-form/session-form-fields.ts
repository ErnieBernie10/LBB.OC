import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FormInput } from '../../components/input-errors/form-input';
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
}
