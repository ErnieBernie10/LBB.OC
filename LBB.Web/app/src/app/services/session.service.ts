import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import {
  CreateSessionCommand,
  EditSessionCommand,
  GetApiSessionsByIdResponse,
  GetApiSessionsResponse,
  PostApiSessionsData,
} from '../api';
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

  public createSession(session: CreateSessionCommand) {
    const { body, url }: PostApiSessionsData = {
      body: session,
      url: '/api/sessions',
    };

    return this.client.post(this.baseUrl + url, body, {
      headers: { Authorization: 'Bearer ' + this.authService.token },
      withCredentials: true,
    });
  }

  public getSessions(range: Observable<{ start: Date; end: Date }>): Observable<WithState<GetApiSessionsResponse>> {
    return range.pipe(
      mergeMap((range) =>
        this.client
          .get<GetApiSessionsResponse>(
            `${this.baseUrl}/api/sessions?from=${range.start.toISOString()}&until=${range.end.toISOString()}`,
            { headers: { Authorization: 'Bearer ' + this.authService.token } }
          )
          .pipe(
            map(
              (response: GetApiSessionsResponse) =>
                ({ data: response, loading: false }) as WithState<GetApiSessionsResponse>
            ),
            startWith({ loading: true } as WithState<GetApiSessionsResponse>),
            catchError((error: HttpErrorResponse) => {
              if (error.status === 500) {
                return of({
                  error: 'Internal server error',
                  loading: false,
                } as WithState<GetApiSessionsResponse>);
              }
              return of({
                error: error?.message ?? 'Unknown error',
                loading: false,
              } as WithState<GetApiSessionsResponse>);
            })
          )
      )
    );
  }

  public updateSession(id: string, session: EditSessionCommand) {
    return this.client.put(this.baseUrl + `/api/sessions/${id}`, session);
  }

  public getSession(id: string) {
    return this.client.get<GetApiSessionsByIdResponse>(`${this.baseUrl}/api/sessions/${id}`);
  }
}
