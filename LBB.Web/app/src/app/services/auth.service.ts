import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthenticatedResponseDto, PostLoginData, PostRefreshData } from '../api';
import { tap } from 'rxjs';
import { PlatformLocation } from '@angular/common';

interface User {
  email: string;
  fullName: string;
  tenantId: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private client: HttpClient = inject(HttpClient);
  private platformLocation: PlatformLocation = inject(PlatformLocation);
  private baseUrl: string = this.platformLocation.getBaseHrefFromDOM();

  public token: string = '';
  public expiration: Date = new Date();
  public user: User | undefined;
  public get isAuthenticated(): boolean {
    return this.token !== '';
  }

  public login(email: string, password: string) {
    const { body, url }: PostLoginData = {
      body: { email, password },
      url: '/login',
    };

    return this.client
      .post<AuthenticatedResponseDto>(this.baseUrl + url, body, { withCredentials: true })
      .pipe(tap(this.setAuthenticationState));
  }

  public refreshToken() {
    const { url }: PostRefreshData = {
      url: '/refresh',
    };

    return this.client
      .post<AuthenticatedResponseDto>(
        this.baseUrl + url,
        {},
        {
          withCredentials: true,
        }
      )
      .pipe(tap(this.setAuthenticationState));
  }

  // Preserve `this` so token/expiration/user are set correctly
  private setAuthenticationState = (p: AuthenticatedResponseDto) => {
    this.user = {
      email: p.email ?? '',
      tenantId: p.tenant?.id ?? '',
      fullName: p.fullName ?? '',
    };
    this.token = p.token!;
    this.expiration = new Date(p.tokenExpiry!);
  };
}
