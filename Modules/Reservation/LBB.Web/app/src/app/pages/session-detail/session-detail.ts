import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { SessionFormFieldsComponent } from '../../components/session-form/session-form-fields';
import { SessionService } from '../../services/session.service';
import { toFormDate } from '../../util/dateutils';

@Component({
  selector: 'app-session-detail-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, SessionFormFieldsComponent, RouterLink],
  templateUrl: './session-detail.html',
})
export class SessionDetailPage {
  private route = inject(ActivatedRoute);
  private sessionService = inject(SessionService);

  session = this.sessionService.getSession(Number(this.route.snapshot.paramMap.get('id')!));
  reservations = this.sessionService.getSessionReservations(Number(this.route.snapshot.paramMap.get('id')!));
  saving = signal<boolean>(false);
  editMode = signal<boolean>(false);

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

  enableEdit() {
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

  save() {
    if (!this.form.dirty) return;
    const value = this.form.getRawValue();
    if (!value.id) return;
    this.saving.set(true);
    this.sessionService.updateSessionInfo(value.id, value).subscribe({
      complete: () => {
        this.saving.set(false);
        this.editMode.set(false);
      },
      error: () => this.saving.set(false),
    });
  }
}
