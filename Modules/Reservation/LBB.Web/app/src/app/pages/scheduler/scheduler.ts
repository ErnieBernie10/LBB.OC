import { Component, inject, signal } from '@angular/core';
import { Appointment, Scheduler as SchedulerC } from '../../components/scheduler/scheduler';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { SessionService, WithState } from '../../services/session.service';
import { Observable, take } from 'rxjs';
import { toFormDate } from '../../util/dateutils';
import { FormInput } from '../../components/input-errors/form-input';
import { Modal, ModalContent, ModalFooter, ModalHeader } from '../../components/modal/modal';
import { InvalidPipe } from '../../pipes/invalid-pipe';
import { Alert } from '../../components/alert/alert';
import { setServerErrors } from '../../util/formutils';

@Component({
  selector: 'app-scheduler-page',
  imports: [
    ReactiveFormsModule,
    FormInput,
    ModalContent,
    Modal,
    ModalHeader,
    InvalidPipe,
    ModalFooter,
    Alert,
    SchedulerC,
  ],
  templateUrl: './scheduler.html',
  styleUrl: './scheduler.scss',
})
export class Scheduler {
  private sessionService = inject(SessionService);
  public savingSession = false;

  public showModal = false;
  public currentWeek = signal({ start: new Date(), end: new Date() });

  public appointments = this.sessionService.getSessions(this.currentWeek);

  public appointments$: Observable<WithState<Appointment[]>> = new Observable<WithState<Appointment[]>>(() => {});

  public appointmentForm = new FormBuilder().group({
    title: [''],
    description: [''],
    end: [''],
    start: [''],
    capacity: [12],
    type: ['Individual'],
    location: [''],
    id: [undefined as number | undefined],
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
    if (!this.appointmentForm.dirty) return;
    this.savingSession = true;
    const value = this.appointmentForm.getRawValue();
    if (value.id) {
      this.sessionService.updateSession(value.id, value).subscribe(this.finalize);
    } else {
      this.sessionService
        .createSession({
          ...value,
          type: value.type === 'Individual' ? 'Individual' : 'Group',
        })
        .subscribe(this.finalize);
    }
  }

  private finalize = {
    error: setServerErrors(this.appointmentForm, () => {
      this.savingSession = false;
    }),
    complete: () => {
      this.showModal = false;
      this.savingSession = false;
      this.appointmentForm.reset();
    },
  };

  onClose() {
    this.showModal = false;
    this.appointmentForm.reset();
  }

  loadAppointments($event: { start: Date; end: Date }) {
    this.currentWeek.set($event);
  }

  onAppointmentUpdate($event: { id: number; start: Date; end: Date }) {
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
          reservations: 0,
          capacity: 12,
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
