import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-csrf',
  imports: [],
  templateUrl: './csrf.html',
  styleUrl: './csrf.scss',
})
export class Csrf {
  authService: AuthService = inject(AuthService);
}
