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
    const qp = this.activatedRoute.snapshot.queryParamMap;
    const startParam = qp.get('start');
    const endParam = qp.get('end');

    if (startParam && endParam) {
      const start = new Date(startParam);
      const end = new Date(endParam);
      if (!isNaN(start.getTime()) && !isNaN(end.getTime())) {
        this.currentWeek.set({ start, end });
      }
    } else {
      // Ensure the current week is reflected in the URL on initial load
      this.router.navigate([], {
        relativeTo: this.activatedRoute,
        queryParams: {
          start: this.currentWeek().start.toISOString(),
          end: this.currentWeek().end.toISOString(),
        },
        queryParamsHandling: 'merge',
      });
    }
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
    // Keep the query string in sync with the current week selection
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: {
        start: this.currentWeek().start.toISOString(),
        end: this.currentWeek().end.toISOString(),
      },
      queryParamsHandling: 'merge',
    });
  }

  onAppointmentUpdate($event: { id: number; start: Date; end: Date }) {
    // Navigate to session detail page for editing and reservations overview
    this.router.navigate(['/sessions', $event.id], {
      queryParams: {
        start: this.currentWeek().start.toISOString(),
        end: this.currentWeek().end.toISOString(),
      },
      queryParamsHandling: 'merge',
    });
  }

  protected readonly Errors = Errors;
}
