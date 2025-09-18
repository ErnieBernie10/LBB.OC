import { inject, Injectable, Signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { AddReservationCommand, Client, CreateSessionCommand, UpdateSessionInfoCommand } from '../api/api';
import { rxResource } from '@angular/core/rxjs-interop';

export type SessionType = 'Individual' | 'Group';

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
  private api = inject(Client);
  private baseUrl = '/reservation/';

  public createSession(session: CreateSessionCommand) {
    return this.api.sessionsPOST(session);
  }
  public updateSessionInfo(id: number, session: UpdateSessionInfoCommand) {
    return this.api.sessionsPATCH(id, session);
  }

  public getSessions(currentWeek: Signal<{ start: Date; end: Date }>) {
    return rxResource({
      params: () => currentWeek(),
      stream: ({ params: { start, end } }) => this.api.sessionsAll(start, end),
    });
  }

  public getSession(id: number) {
    return rxResource({
      params: () => id,
      stream: ({ params: id }) => this.api.sessionsGET(id),
    });
  }

  public getSessionReservations(sessionId: number) {
    return rxResource({
      params: () => ({ sessionId }),
      stream: ({ params }) => this.api.reservationsAll(params.sessionId),
    });
  }

  public addReservation(sessionId: number, command: AddReservationCommand) {
    return this.api.reservations(sessionId, command);
  }
}
