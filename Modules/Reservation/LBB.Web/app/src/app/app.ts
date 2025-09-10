import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { Csrf } from './components/csrf/csrf';
import { AlertToastsComponent } from './components/alert/alert-toasts';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, Csrf, AlertToastsComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  authService = inject(AuthService);
  protected title = 'app';
}
