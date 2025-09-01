import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PlatformLocation } from '@angular/common';
import { firstValueFrom } from 'rxjs';

interface User {
  email: string;
  username: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private client: HttpClient = inject(HttpClient);
  private platformLocation: PlatformLocation = inject(PlatformLocation);
  private baseUrl: string = this.platformLocation.getBaseHrefFromDOM();

  public user: User | undefined;

  public async setCurrentUser() {
    try {
      this.user = await firstValueFrom(this.client.get<User>(`${this.baseUrl}users/current`));
    } catch (error) {
      console.error(error);
    }
  }
}
