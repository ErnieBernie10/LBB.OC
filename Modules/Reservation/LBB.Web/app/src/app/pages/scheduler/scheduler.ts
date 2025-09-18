import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Appointment, Scheduler as SchedulerC } from '../../components/scheduler/scheduler';
import { SessionFormFieldsComponent } from '../../components/session-form/session-form-fields';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { SessionService } from '../../services/session.service';
import { toFormDate } from '../../util/dateutils';
import { Modal, ModalContent, ModalFooter, ModalHeader } from '../../components/modal/modal';
import { Alert } from '../../components/alert/alert';
import { FormValidationService } from '../../services/form-validation.service';
import { Errors } from '../../models/errors';
import { CreateSessionCommand, UpdateSessionInfoCommand } from '../../api/api';

@Component({
  selector: 'app-scheduler-page',
  imports: [
    ReactiveFormsModule,
    ModalContent,
    Modal,
    ModalHeader,
    ModalFooter,
    Alert,
    SchedulerC,
    SessionFormFieldsComponent,
  ],
  templateUrl: './scheduler.html',
  styleUrl: './scheduler.scss',
})
export class Scheduler implements OnInit {
  private sessionService = inject(SessionService);
  private formService = inject(FormValidationService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  public savingSession = false;
  public isEditing = false;

  public showModal = false;
  public currentWeek = signal({ start: new Date(), end: new Date() });

  ngOnInit(): void {
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: {
        start: this.currentWeek().start.toISOString(),
        end: this.currentWeek().end.toISOString(),
      },
      queryParamsHandling: 'merge',
    });
  }

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
    this.isEditing = false;
    this.appointmentForm.get('start')?.setValue(toFormDate($event.start));
    this.appointmentForm.get('end')?.setValue(toFormDate($event.end));
    this.showModal = true;
  }

  handleSubmit() {
    if (!this.appointmentForm.dirty) return;
    this.savingSession = true;
    const value = this.appointmentForm.getRawValue();
    if (value.id) {
      this.sessionService
        .updateSessionInfo(
          value.id,
          new UpdateSessionInfoCommand({
            ...value,
            sessionId: value.id,
            start: new Date(value.start),
            end: new Date(value.end),
          })
        )
        .subscribe(this.finalize);
    } else {
      this.sessionService
        .createSession(
          new CreateSessionCommand({
            ...value,
            type: value.type === 'Individual' ? 'Individual' : 'Group',
            start: new Date(value.start),
            end: new Date(value.end),
          })
        )
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
    // Navigate to session detail page for editing and reservations overview
    this.router.navigate(['/sessions', $event.id]);
  }

  protected readonly Errors = Errors;
}
