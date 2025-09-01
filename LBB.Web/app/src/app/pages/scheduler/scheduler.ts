import { Component, inject } from '@angular/core';
import { Appointment, Scheduler as SchedulerC } from '../../components/scheduler/scheduler';
import { Modal, ModalContent, ModalFooter, ModalHeader } from '../../components/modal/modal';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { InvalidPipe } from '../../pipes/invalid-pipe';
import { CreateSessionCommandSchema } from '../../api/schemas.gen';
import { buildValidations } from '../../validation/buildValidations';
import { FormInput } from '../../components/input-errors/form-input';
import { SessionService, WithState } from '../../services/session.service';
import { BehaviorSubject, map, Observable, take } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { switchMap } from 'rxjs/operators';
import { Alert } from '../../components/alert/alert';
import { toFormDate } from '../../util/dateutils';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-scheduler-page',
  imports: [
    SchedulerC,
    Modal,
    ModalFooter,
    ModalHeader,
    ModalContent,
    ReactiveFormsModule,
    InvalidPipe,
    FormInput,
    AsyncPipe,
    Alert,
  ],
  templateUrl: './scheduler.html',
  styleUrl: './scheduler.scss',
})
export class Scheduler {
  private sessionService = inject(SessionService);
  public savingSession = false;

  public showModal = false;
  public currentWeek = new BehaviorSubject<{ start: Date; end: Date }>({ start: new Date(), end: new Date() });
  private refreshTrigger = new BehaviorSubject<void>(undefined);
  public authService = inject(AuthService);

  public appointments$: Observable<WithState<Appointment[]>> = this.refreshTrigger.pipe(
    switchMap(() => this.sessionService.getSessions(this.currentWeek)),
    map((sessions) => {
      return {
        ...sessions,
        data: (sessions.data?.items ?? []).map(
          (session) =>
            ({
              id: session.id,
              start: new Date(session.start),
              end: new Date(session.end),
              title: session.title,
              description: session.description,
            }) as Appointment
        ),
      };
    })
  );

  private formBuilder = new FormBuilder().nonNullable;
  public appointmentForm = this.formBuilder.group({
    id: [undefined as string | undefined],
    title: ['', buildValidations('title', CreateSessionCommandSchema)],
    description: ['', buildValidations('description', CreateSessionCommandSchema)],
    location: ['', buildValidations('location', CreateSessionCommandSchema)],
    start: ['', buildValidations('start', CreateSessionCommandSchema)],
    end: ['', buildValidations('end', CreateSessionCommandSchema)],
    type: ['Individual', buildValidations('type', CreateSessionCommandSchema)],
    capacity: [12, buildValidations('capacity', CreateSessionCommandSchema)],
  });

  public get title() {
    return this.appointmentForm.get('title');
  }
  public get description() {
    return this.appointmentForm.get('description');
  }
  public get location() {
    return this.appointmentForm.get('location');
  }
  public get start() {
    return this.appointmentForm.get('start');
  }
  public get end() {
    return this.appointmentForm.get('end');
  }
  public get type() {
    return this.appointmentForm.get('type');
  }
  public get capacity() {
    return this.appointmentForm.get('capacity');
  }

  onAppointmentCreate($event: { start: Date; end: Date }) {
    this.appointmentForm.get('start')?.setValue(toFormDate($event.start));
    this.appointmentForm.get('end')?.setValue(toFormDate($event.end));
    this.showModal = true;
  }

  handleSubmit() {
    if (!this.appointmentForm.valid) {
      this.appointmentForm.markAllAsDirty();
      return;
    }
    this.savingSession = true;
    const value = this.appointmentForm.getRawValue();
    if (value.id) {
      this.sessionService.updateSession(value.id, value).subscribe({
        error: () => {
          this.showModal = false;
          this.savingSession = false;
          this.refreshTrigger.next();
        },
        complete: () => {
          this.showModal = false;
          this.savingSession = false;
          this.appointmentForm.reset();
          this.refreshTrigger.next();
        },
      });
    } else {
      this.sessionService
        .createSession({
          ...value,
          type: value.type === 'Individual' ? 'Individual' : 'Group',
          tenantId: this.authService.user!.tenantId,
        })
        .subscribe({
          error: () => {
            this.showModal = false;
            this.savingSession = false;
            this.refreshTrigger.next();
          },
          complete: () => {
            this.showModal = false;
            this.savingSession = false;
            this.appointmentForm.reset();
            this.refreshTrigger.next();
          },
        });
    }
  }

  onClose() {
    this.showModal = false;
    this.appointmentForm.reset();
  }

  loadAppointments($event: { start: Date; end: Date }) {
    this.currentWeek.next($event);
  }

  onAppointmentUpdate($event: { id: string; start: Date; end: Date }) {
    this.appointments$.pipe(take(1)).subscribe((state) => {
      const session = (state.data ?? []).find((a) => a.id === $event.id);
      const selected =
        session ??
        ({
          id: $event.id,
          title: '',
          description: '',
          start: $event.start,
          end: $event.end,
        } as Appointment);

      this.appointmentForm.setValue({
        title: selected.title ?? '',
        description: selected.title ?? '',
        end: toFormDate(new Date(selected.end)),
        start: toFormDate(new Date(selected.start)),
        capacity: (selected as any).capacity ?? 12,
        type: (selected as any).type ?? 'Individual',
        location: (selected as any).location ?? '',
        id: selected.id,
      });
      this.showModal = true;
    });
  }
}
