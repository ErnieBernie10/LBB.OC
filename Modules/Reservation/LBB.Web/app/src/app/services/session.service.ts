import { inject, Injectable, Signal } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { AuthService } from './auth.service';

export type SessionType = 'Individual' | 'Group';

export interface Session {
  capacity: number;
  attendeeCount: number;
  id: number;
  title: string;
  description: string;
  start: Date;
  end: Date;
  type: SessionType;
  location: string;
}

export interface CreateSession {
  capacity: number;
  title: string;
  description: string;
  start: string;
  end: string;
  type: SessionType;
}
@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private client: HttpClient = inject(HttpClient);
  private authService: AuthService = inject(AuthService);
  private baseUrl = '/reservation/';

  public createSession(session: CreateSession) {
    return this.client.post(`${this.baseUrl}sessions`, session, {
      withCredentials: true,
      headers: {
        RequestVerificationToken: this.authService.getXsrfToken(),
      },
    });
  }
  public updateSessionInfo(
    id: number,
    session: {
      title: string;
      description: string;
      capacity: number;
      start: string;
      end: string;
    }
  ) {
    return this.client.patch(this.baseUrl + `sessions/${id}`, session, {
      withCredentials: true,
      headers: {
        RequestVerificationToken: this.authService.getXsrfToken(),
      },
    });
  }

  public getSessions(currentWeek: Signal<{ start: Date; end: Date }>) {
    return httpResource<Session[]>(
      () => `${this.baseUrl}sessions?start=${currentWeek().start.toISOString()}&end=${currentWeek().end.toISOString()}`,
      {
        parse: (sessions) => {
          return (sessions as Session[]).map(
            (s): Session => ({
              start: new Date(s.start),
              end: new Date(s.end),
              id: s.id,
              title: s.title,
              description: s.description,
              attendeeCount: s.attendeeCount,
              type: s.type,
              capacity: s.capacity,
              location: s.location,
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
