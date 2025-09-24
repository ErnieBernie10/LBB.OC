import { inject, Injectable, Signal } from '@angular/core';
import { AddReservationCommand, Client, CreateSessionCommand, UpdateSessionInfoCommand } from '../api/api';
import { rxResource } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class SessionService {
  private api = inject(Client);

  public createSession(session: CreateSessionCommand) {
    return this.api.sessionsPOST(session);
  }
  public updateSessionInfo(id: number, session: UpdateSessionInfoCommand) {
    return this.api.sessionsPATCH(id, session);
  }
  public deleteSession(id: number) {
    return this.api.sessionsDELETE(id);
  }
  public cancelSession(id: number) {
    return this.api.cancel(id);
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
    return this.api.reservationsPOST(sessionId, command);
  }
}
