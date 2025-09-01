import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { PlatformLocation } from '@angular/common';
import { firstValueFrom } from 'rxjs';

interface User {
  email: string;
  username: string;
}

interface CsrfToken {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private client: HttpClient = inject(HttpClient);
  private platformLocation: PlatformLocation = inject(PlatformLocation);
  private baseUrl: string = this.platformLocation.getBaseHrefFromDOM();

  private xsrfToken: string = '';
  public getXsrfToken(): string {
    return this.xsrfToken;
  }
  public user: User | undefined;

  // Single-flight guard to avoid parallel refreshes
  private refreshPromise: Promise<void> | null = null;

  // Make this public so interceptor can refresh the token
  public async setXsrfToken(): Promise<void> {
    if (this.refreshPromise) {
      return this.refreshPromise;
    }
    this.refreshPromise = firstValueFrom(this.client.get<CsrfToken>(`${this.baseUrl}csrf`))
      .then((t) => {
        this.xsrfToken = t.token;
      })
      .finally(() => {
        this.refreshPromise = null;
      });
    return this.refreshPromise;
  }

  public async init() {
    try {
      this.user = await firstValueFrom(this.client.get<User>(`${this.baseUrl}users/current`));
      await this.setXsrfToken();
    } catch (error) {
      console.error(error);
    }
  }
}
