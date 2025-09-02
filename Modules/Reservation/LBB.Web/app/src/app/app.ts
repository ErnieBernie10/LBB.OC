import { Component, inject } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { Csrf } from './components/csrf/csrf';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, Csrf],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  authService = inject(AuthService);
  protected title = 'app';
}
