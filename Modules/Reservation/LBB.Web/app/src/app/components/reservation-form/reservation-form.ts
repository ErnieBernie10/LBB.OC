import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { FormInput } from '../form-input/form-input';
import { InvalidPipe } from '../../pipes/invalid-pipe';
import { Errors } from '../../models/errors';

@Component({
  selector: 'app-reservation-form',
  imports: [ReactiveFormsModule, FormInput, InvalidPipe],
  templateUrl: './reservation-form.html',
  styleUrl: './reservation-form.scss',
})
export class ReservationForm {
  @Input({ required: true }) form!: FormGroup;

  get firstname() {
    return this.form.get('firstname');
  }
  get lastname() {
    return this.form.get('lastname');
  }
  get email() {
    return this.form.get('email');
  }
  get phoneNumber() {
    return this.form.get('phoneNumber');
  }
  get attendeeCount() {
    return this.form.get('attendeeCount');
  }

  protected readonly Errors = Errors;
}
