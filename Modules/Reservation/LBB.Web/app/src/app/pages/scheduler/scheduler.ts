import { Component, computed, inject, signal } from '@angular/core';
import { Appointment, Scheduler as SchedulerC } from '../../components/scheduler/scheduler';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { SessionService } from '../../services/session.service';
import { toFormDate } from '../../util/dateutils';
import { FormInput } from '../../components/input-errors/form-input';
import { Modal, ModalContent, ModalFooter, ModalHeader } from '../../components/modal/modal';
import { InvalidPipe } from '../../pipes/invalid-pipe';
import { Alert } from '../../components/alert/alert';
import { FormValidationService } from '../../services/form-validation.service';
import { Errors } from '../../models/errors';
import { CreateSessionCommand, ICreateSessionCommand, UpdateSessionInfoCommand } from '../../api/api';

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
  private formService = inject(FormValidationService);
  public savingSession = false;
  public isEditing = false;

  public showModal = false;
  public currentWeek = signal({ start: new Date(), end: new Date() });

  public sessions = this.sessionService.getSessions(this.currentWeek);
  public appointments = computed(() => {
    return (
      this.sessions.value()?.map(
        (s): Appointment => ({
          id: s.id,
          title: s.title ?? '',
          description: s.description ?? '',
          start: new Date(s.start),
          end: new Date(s.end),
          capacity: s.capacity,
          reservations: s.attendeeCount,
        })
      ) ?? []
    );
  });

  public appointmentForm = new FormBuilder().nonNullable.group({
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
      this.sessionService.updateSessionInfo(value.id, value).subscribe(this.finalize);
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
    error: this.formService.setServerErrors(this.appointmentForm, () => {
      this.savingSession = false;
    }),
    complete: () => {
      this.showModal = false;
      this.savingSession = false;
      this.appointmentForm.reset();
      this.sessions.reload();
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
    const value = this.sessions.value();

    const session = value?.find((a) => a.id === $event.id);
    const selected: ICreateSessionCommand = {
      ...session,
      title: session?.title ?? '',
      description: session?.description ?? '',
      start: session?.start ?? new Date(),
      end: session?.end ?? new Date(),
      capacity: session?.capacity ?? 12,
      location: session?.location ?? '',
      type: session?.type === 'Individual' ? 'Individual' : 'Group',
    };
    console.log(selected);

    this.appointmentForm.setValue({
      title: selected.title ?? '',
      description: selected.description ?? '',
      end: toFormDate(new Date(selected.end)),
      start: toFormDate(new Date(selected.start)),
      capacity: selected.capacity ?? 12,
      type: selected.type ?? 'Individual',
      location: selected.location ?? '',
      id: 0,
    });
    this.isEditing = true;
    this.showModal = true;
  }

  protected readonly Errors = Errors;
}
