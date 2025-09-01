import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import {
  CreateSessionCommand,
  EditSessionCommand,
  GetApiSessionsByIdResponse,
  GetApiSessionsResponse,
  PostApiSessionsData,
} from '../api';
import { clientOptions } from '../client';
import { catchError, map, mergeMap, Observable, of, startWith } from 'rxjs';
import { AuthService } from './auth.service';

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

  public createSession(session: CreateSessionCommand) {
    const { body, url }: PostApiSessionsData = {
      body: session,
      url: '/api/sessions',
    };

    return this.client.post(clientOptions.baseUrl + url, body, {
      headers: { Authorization: 'Bearer ' + this.authService.token },
      withCredentials: true,
    });
  }

  public getSessions(range: Observable<{ start: Date; end: Date }>): Observable<WithState<GetApiSessionsResponse>> {
    return range.pipe(
      mergeMap((range) =>
        this.client
          .get<GetApiSessionsResponse>(
            `${clientOptions.baseUrl}/api/sessions?from=${range.start.toISOString()}&until=${range.end.toISOString()}`,
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
    return this.client.put(clientOptions.baseUrl + `/api/sessions/${id}`, session);
  }

  public getSession(id: string) {
    return this.client.get<GetApiSessionsByIdResponse>(`${clientOptions.baseUrl}/api/sessions/${id}`);
  }
}
