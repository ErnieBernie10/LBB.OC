import { inject, Injectable, Signal } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { AuthService } from './auth.service';
import { PlatformLocation } from '@angular/common';
import { Appointment } from '../components/scheduler/scheduler';

export interface WithState<T> {
  data?: T;
  error?: string;
  loading: boolean;
}

export interface Session {
  capacity: number;
  attendeeCount: number;
  id: number;
  title: string;
  description: string;
  start: Date;
  end: Date;
}
@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private client: HttpClient = inject(HttpClient);
  private authService: AuthService = inject(AuthService);
  private baseUrl = '/reservation/';

  public createSession(session: unknown) {
    return this.client.post(`${this.baseUrl}sessions`, session, {
      withCredentials: true,
      headers: {
        RequestVerificationToken: this.authService.getXsrfToken(),
      },
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
              reservations: s.attendeeCount,
              capacity: s.capacity,
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
