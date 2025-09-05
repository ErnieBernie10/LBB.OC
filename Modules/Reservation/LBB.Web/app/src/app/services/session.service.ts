import { inject, Injectable, Signal } from '@angular/core';
import { HttpClient, HttpErrorResponse, httpResource } from '@angular/common/http';
import { catchError, map, mergeMap, Observable, of, startWith } from 'rxjs';
import { AuthService } from './auth.service';
import { PlatformLocation } from '@angular/common';
import { Appointment } from '../components/scheduler/scheduler';
import { FormBuilder, Validators } from '@angular/forms';

export interface WithState<T> {
  data?: T;
  error?: string;
  loading: boolean;
}

export interface Session {
  id: number;
  title: string;
  description: string;
  start: Date;
  end: Date;
}

export type SessionType = 'Individual' | 'Group';

export interface CreateSession {
  title: string;
  description: string;
  start: string;
  end: string;
  type: SessionType;
  location: string;
  capacity: number;
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
    return this.client.post(`${this.baseUrl}sessions`, session, {
      withCredentials: true,
      headers: {
        RequestVerificationToken: this.authService.getXsrfToken(),
      },
    });
  }

  public makeSessionForm() {
    return new FormBuilder().nonNullable.group({
      id: [undefined as number | undefined],
      start: ['', Validators.required],
      end: ['', Validators.required],
      title: ['', Validators.required],
      description: [''],
      type: ['Individual', Validators.required],
      location: [''],
      capacity: [12, Validators.required],
    });
  }

  public updateSession(id: number, session: unknown) {
    return this.client.put(this.baseUrl + `/api/sessions/${id}`, session);
  }

  public getSessions(currentWeek: Signal<{ start: Date; end: Date }>) {
    return httpResource<Appointment[]>(
      () => `${this.baseUrl}sessions?start=${currentWeek().start.toISOString()}&end=${currentWeek().end.toISOString()}`,
      {
        parse: (sessions) => {
          return (sessions as Session[]).map(
            (s): Appointment => ({
              start: new Date(s.start),
              end: new Date(s.end),
              id: s.id,
              title: s.title,
            })
          );
        },
      }
    );
  }

  public getSession(id: number) {
    return this.client.get<unknown>(`${this.baseUrl}/api/sessions/${id}`);
  }
}
