import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, mergeMap, Observable, of, startWith } from 'rxjs';
import { AuthService } from './auth.service';
import { PlatformLocation } from '@angular/common';

export interface WithState<T> {
  data?: T;
  error?: string;
  loading: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private client: HttpClient = inject(HttpClient);
  private authService: AuthService = inject(AuthService);
  private platformLocation: PlatformLocation = inject(PlatformLocation);
  private baseUrl = this.platformLocation.getBaseHrefFromDOM();

  public createSession(session: unknown) {
    const { body, url } = {
      body: session,
      url: '/api/sessions',
    };

    return this.client.post(this.baseUrl + url, body, {
      withCredentials: true,
    });
  }

  public getSessions(range: Observable<{ start: Date; end: Date }>): Observable<WithState<unknown>> {
    return range.pipe(
      mergeMap((range) =>
        this.client
          .get<unknown>(
            `${this.baseUrl}/api/sessions?from=${range.start.toISOString()}&until=${range.end.toISOString()}`
          )
          .pipe(
            map((response: unknown) => ({ data: response, loading: false }) as WithState<unknown>),
            startWith({ loading: true } as WithState<unknown>),
            catchError((error: HttpErrorResponse) => {
              if (error.status === 500) {
                return of({
                  error: 'Internal server error',
                  loading: false,
                } as WithState<unknown>);
              }
              return of({
                error: error?.message ?? 'Unknown error',
                loading: false,
              } as WithState<unknown>);
            })
          )
      )
    );
  }

  public updateSession(id: string, session: unknown) {
    return this.client.put(this.baseUrl + `/api/sessions/${id}`, session);
  }

  public getSession(id: string) {
    return this.client.get<unknown>(`${this.baseUrl}/api/sessions/${id}`);
  }
}
