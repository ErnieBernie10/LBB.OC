import { inject, Injectable, Signal } from '@angular/core';
import {
  AddReservationCommand,
  Client,
  CreateSessionCommand,
  UpdateReservationCommand,
  UpdateSessionInfoCommand,
} from '../api/api';
import { rxResource } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';

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
      stream: ({ params }) =>
        this.api.reservationsAll(params.sessionId).pipe(
          map((r) =>
            r.sort((a, b) => {
              // First, sort by cancelledOn (active reservations first)
              const aCancelled = a.cancelledOn ? 1 : 0;
              const bCancelled = b.cancelledOn ? 1 : 0;
              if (aCancelled !== bCancelled) {
                return aCancelled - bCancelled;
              }

              // Then sort by lastname
              const lastNameCompare = (a.lastname || '').localeCompare(b.lastname || '');
              if (lastNameCompare !== 0) {
                return lastNameCompare;
              }

              // Then sort by firstname
              const firstNameCompare = (a.firstname || '').localeCompare(b.firstname || '');
              if (firstNameCompare !== 0) {
                return firstNameCompare;
              }

              // Finally sort by email
              return (a.email || '').localeCompare(b.email || '');
            })
          )
        ),
    });
  }

  public addReservation(sessionId: number, command: AddReservationCommand) {
    return this.api.reservationsPOST(sessionId, command);
  }

  public deleteReservation(sessionId: number, reservationId: number) {
    return this.api.reservationsDELETE(sessionId, reservationId);
  }

  public editReservation(sessionId: number, reservationId: number, command: UpdateReservationCommand) {
    return this.api.reservationsPATCH(sessionId, reservationId, command);
  }
}
