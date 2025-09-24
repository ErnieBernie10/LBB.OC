import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { SessionFormFieldsComponent } from '../../components/session-form/session-form-fields';
import { SessionService } from '../../services/session.service';
import { toFormDate } from '../../util/dateutils';
import { AddReservationCommand, IAddReservationCommand, UpdateSessionInfoCommand } from '../../api/api';
import { Modal, ModalContent, ModalFooter, ModalHeader } from '../../components/modal/modal';
import { ReservationForm } from '../../components/reservation-form/reservation-form';
import { FormValidationService } from '../../services/form-validation.service';
import { ConfirmationDialog } from '../../components/confirmation-dialog/confirmation-dialog';
import { DialogService } from '../../services/confirmation-dialog.service';
import { DefaultConfirmationDialog } from '../../constants/i18n-common';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-session-detail-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SessionFormFieldsComponent,
    RouterLink,
    Modal,
    ModalHeader,
    ModalContent,
    ReservationForm,
    ModalFooter,
  ],
  templateUrl: './session-detail.html',
  styleUrls: ['./session-detail.scss'],
})
export class SessionDetailPage {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private sessionService = inject(SessionService);
  private confirmationDialogService = inject(DialogService);

  session = this.sessionService.getSession(Number(this.route.snapshot.paramMap.get('id')!));
  reservations = this.sessionService.getSessionReservations(Number(this.route.snapshot.paramMap.get('id')!));
  saving = signal<boolean>(false);
  savingReservation = signal<boolean>(false);
  editMode = signal<boolean>(false);
  reservation = signal<IAddReservationCommand | null>(null);
  deleting = signal<boolean>(false);
  formService = inject(FormValidationService);

  form = new FormBuilder().nonNullable.group({
    title: [''],
    description: [''],
    end: [''],
    start: [''],
    capacity: [12],
    type: ['Individual'],
    location: [''],
    id: [undefined as number | undefined],
  });
  reservationForm = new FormBuilder().nonNullable.group({
    firstname: [''],
    lastname: [''],
    email: [''],
    phoneNumber: [''],
    attendeeCount: [1],
  });

  enableEdit() {
    const s = this.session.value();
    if (s) {
      this.form.patchValue({
        title: s.title ?? '',
        description: s.description ?? '',
        end: toFormDate(new Date(s.end)),
        start: toFormDate(new Date(s.start)),
        capacity: s.capacity ?? 12,
        type: s.type ?? 'Individual',
        location: s.location ?? '',
        id: s.id,
      });
    }
    this.editMode.set(true);
  }

  cancelEdit() {
    this.editMode.set(false);
    const s = this.session.value();
    if (s) {
      this.form.patchValue({
        title: s.title ?? '',
        description: s.description ?? '',
        end: toFormDate(new Date(s.end)),
        start: toFormDate(new Date(s.start)),
        capacity: s.capacity ?? 12,
        type: s.type ?? 'Individual',
        location: s.location ?? '',
        id: s.id,
      });
    }
  }

  addReservation() {
    this.reservation.set({
      ...this.reservationForm.getRawValue(),
      sessionId: this.session?.value()?.id ?? -1,
    });
  }

  save() {
    if (!this.form.dirty) return;
    const value = this.form.getRawValue();
    if (!value.id) return;
    this.saving.set(true);
    this.sessionService
      .updateSessionInfo(
        value.id,
        new UpdateSessionInfoCommand({
          ...value,
          sessionId: this.session.value()!.id,
          start: new Date(value.start),
          end: new Date(value.end),
        })
      )
      .subscribe({
        complete: () => {
          this.saving.set(false);
          this.editMode.set(false);
          this.session.reload();
        },
        error: this.formService.setServerErrors(this.form, () => {
          this.saving.set(false);
        }),
      });
  }

  saveReservation() {
    if (!this.reservationForm.dirty) return;

    const value = this.reservationForm.getRawValue();
    this.savingReservation.set(true);
    this.sessionService
      .addReservation(
        this.session.value()!.id,
        new AddReservationCommand({
          ...value,
          sessionId: this.session.value()!.id,
        })
      )
      .subscribe({
        complete: () => {
          this.savingReservation.set(false);
          this.reservationForm.reset();
          this.reservations.reload();
        },
        error: this.formService.setServerErrors(this.reservationForm, () => {
          this.savingReservation.set(false);
        }),
      });
  }

  openDeleteConfirm() {
    this.confirmationDialogService
      .open(
        ConfirmationDialog,
        {
          ...DefaultConfirmationDialog,
          message: $localize`Are you sure you want to delete this session? This action is irreversible.`,
        },
        this.performDelete()
      )
      .subscribe({
        complete: () => {
          this.deleting.set(false);
          this.router.navigateByUrl('/scheduler');
        },
        error: () => {
          this.deleting.set(false);
        },
      });
  }

  cancelSession() {
    this.confirmationDialogService
      .open(
        ConfirmationDialog,
        {
          ...DefaultConfirmationDialog,
          message: $localize`Are you sure you want to cancel this session? All participants will be notified. And their reservation will be cancelled.`,
        },
        this.performCancel()
      )
      .subscribe({
        complete: () => {
          this.deleting.set(false);
          this.session.reload();
        },
        error: () => {
          this.deleting.set(false);
        },
      });
  }

  performDelete() {
    const id = this.session.value()?.id;
    if (!id) return;
    this.deleting.set(true);
    return this.sessionService.deleteSession(id);
  }

  performCancel() {
    const id = this.session.value()?.id;
    if (!id) return;
    this.deleting.set(true);
    return this.sessionService.cancelSession(id);
  }
}
