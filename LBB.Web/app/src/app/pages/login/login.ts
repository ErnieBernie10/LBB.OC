import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { LoginRequestSchema } from '../../api/schemas.gen';
import { buildValidations } from '../../validation/buildValidations';
import { AuthService } from '../../services/auth.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { FormInput } from '../../components/input-errors/form-input';
import { Alert } from '../../components/alert/alert';
import { InvalidPipe } from '../../pipes/invalid-pipe';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, FormInput, Alert, InvalidPipe],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  private authService = inject(AuthService);
  private formBuilder = new FormBuilder().nonNullable;
  private router = inject(Router);
  public loginForm = this.formBuilder.group({
    email: ['', buildValidations('email', LoginRequestSchema)],
    password: ['', buildValidations('password', LoginRequestSchema)],
  });

  public loading = false;
  public error: string | null = null;
  public success: boolean = false;

  onSubmit() {
    if (!this.loginForm.valid) {
      this.loginForm.markAllAsDirty();
      return;
    }
    const value = this.loginForm.getRawValue();
    this.loading = true;

    this.authService.login(value.email, value.password).subscribe({
      error: (e: HttpErrorResponse) => {
        this.loading = false;
        this.success = false;
        if (e.status === 401) {
          this.error = 'Invalid email or password';
          return;
        }
        this.error = 'Something went wrong. Please try again later.';
      },
      next: (response) => {
        this.loading = false;
        this.error = null;
        this.success = true;
        setTimeout(() => this.router.navigateByUrl('/'), 500);
      },
    });
  }

  public get email() {
    return this.loginForm.get('email');
  }

  public get password() {
    return this.loginForm.get('password');
  }
}
