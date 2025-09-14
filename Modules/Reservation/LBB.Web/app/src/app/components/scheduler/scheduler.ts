import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  inject,
  Input,
  LOCALE_ID,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { addDays, addWeeks, format, formatDate, setHours, setMinutes, startOfWeek, subWeeks } from 'date-fns';
import { nlBE, enUS } from 'date-fns/locale';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { withDelayedLoading } from '../../operators/withDelayedLoading';

export interface Appointment {
  reservations: number;
  capacity: number;
  description: string;
  id: number;
  title: string;
  start: Date;
  end: Date;
}

@Component({
  selector: 'app-scheduler',
  templateUrl: './scheduler.html',
  imports: [CommonModule],
  standalone: true,
  styleUrls: ['./scheduler.scss'],
})
export class Scheduler implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('scrollContainer') scrollContainer!: ElementRef;
  @Input() appointments: Appointment[] = [];

  private loadingSubject = new BehaviorSubject<boolean>(false);
  private destroy$ = new Subject<void>();
  private locale = inject(LOCALE_ID);
  l = this.locale === 'nl' ? nlBE : enUS;

  @Input()
  set loading(value: boolean) {
    this.loadingSubject.next(value);
  }

  showLoading$: Observable<boolean> = withDelayedLoading(this.loadingSubject);

  @Output() appointmentCreate = new EventEmitter<{ start: Date; end: Date }>();
  @Output() appointmentUpdate = new EventEmitter<{ id: number; start: Date; end: Date }>();
  @Output() loadAppointments = new EventEmitter<{ start: Date; end: Date }>();

  currentWeek: Date = new Date();
  weekDays: Date[] = [];
  timeSlots: string[] = [];

  ngOnInit() {
    this.generateTimeSlots();
    this.updateWeekDays();
  }

  workHourStart = 8;
  workHourEnd = 19;
  pixelsPerHour = 60; // Increased from 30 to 60 for better precision

  generateTimeSlots() {
    // Generate 15-minute slots (96 per day), labels shown only on full hours in template
    this.timeSlots = Array.from({ length: 96 }, (_, i) => {
      const hours = Math.floor(i / 4);
      const minutes = (i % 4) * 15;
      const date = setMinutes(setHours(new Date(), hours), minutes);
      return format(date, 'HH:mm', { locale: this.l });
    });
  }

  updateWeekDays() {
    const start = startOfWeek(this.currentWeek, { weekStartsOn: 1 });
    this.loadAppointments.emit({ start: start, end: addWeeks(start, 1) });
    this.weekDays = Array.from({ length: 7 }, (_, i) => addDays(start, i));
  }

  previousWeek() {
    this.currentWeek = subWeeks(this.currentWeek, 1);
    this.updateWeekDays();
  }

  nextWeek() {
    this.currentWeek = addWeeks(this.currentWeek, 1);
    this.updateWeekDays();
  }

  getAppointmentStyle(appointment: Appointment): { [key: string]: string } {
    const startHour = appointment.start.getHours();
    const startMinute = appointment.start.getMinutes();
    const top = (startHour * 60 + startMinute) * (this.pixelsPerHour / 60);
    const duration =
      appointment.end.getHours() * 60 +
      appointment.end.getMinutes() -
      (appointment.start.getHours() * 60 + appointment.start.getMinutes());
    return {
      top: `${top}px`,
      height: `${duration * (this.pixelsPerHour / 60)}px`,
    };
  }

  getAppointmentsForDay(day: Date): Appointment[] {
    return this.appointments.filter((apt) => format(apt.start, 'yyyy-MM-dd') === format(day, 'yyyy-MM-dd'));
  }

  isWorkHour(time: string): boolean {
    const hour = parseInt(time.split(':')[0], 10);
    return hour >= this.workHourStart && hour <= this.workHourEnd;
  }

  isFullHour(time: string): boolean {
    // time is in format HH:mm
    return time.endsWith(':00');
  }

  getWeekStart(): Date {
    return startOfWeek(this.currentWeek, { weekStartsOn: 1 });
  }

  getWeekEnd(): Date {
    return addDays(this.getWeekStart(), 6);
  }

  protected readonly format = format;

  ngAfterViewInit() {
    // Scroll to work hours on initial load
    const scrollOffset = this.workHourStart * this.pixelsPerHour;
    this.scrollContainer.nativeElement.scrollTop = scrollOffset;
  }

  onColumnClick(event: MouseEvent, day: Date, container: HTMLElement) {
    // First, check if we clicked directly on an event element
    const eventElement = (event.target as HTMLElement).closest('.event') as HTMLElement | null;
    if (eventElement) {
      // Prefer a direct mapping using a data attribute for reliability
      const idAttr = eventElement.getAttribute('data-appointment-id');
      if (idAttr) {
        const id = Number(idAttr);
        const existingAppointment = this.getAppointmentsForDay(day).find((a) => a.id === id);
        if (existingAppointment) {
          this.appointmentUpdate.emit({
            id: existingAppointment.id,
            start: existingAppointment.start,
            end: existingAppointment.end,
          });
          return;
        }
      }

      // Fallback: keep old behavior but limit to the day column
      const appointments = this.getAppointmentsForDay(day);
      const rect = eventElement.getBoundingClientRect();
      const existingAppointment = appointments.find(() => {
        const eventTop = rect.top;
        const eventBottom = rect.bottom;
        const clickY = event.clientY;
        return clickY >= eventTop && clickY <= eventBottom;
      });
      if (existingAppointment) {
        this.appointmentUpdate.emit({
          id: existingAppointment.id,
          start: existingAppointment.start,
          end: existingAppointment.end,
        });
        return;
      }
    }

    // If we didn't click on an event, proceed with creating a new appointment
    const rect = container.getBoundingClientRect();
    // Adjust for the sticky header (40px) and padding-top we add to events-container
    const headerOffset = 40;
    const y = event.clientY - rect.top + container.scrollTop - headerOffset;

    const totalMinutes = Math.max(0, Math.floor((y * 60) / this.pixelsPerHour));
    let hours = Math.floor(totalMinutes / 60);
    let minutes = Math.round((totalMinutes % 60) / 15) * 15;

    if (minutes === 60) {
      minutes = 0;
      hours += 1;
    }
    // clamp to day bounds
    if (hours < 0) hours = 0;
    if (hours > 23) hours = 23;

    const startDate = setMinutes(setHours(day, hours), minutes);
    // Default duration 15 minutes
    const endDate = setMinutes(setHours(day, hours), minutes === 45 ? 0 : minutes + 15);
    this.appointmentCreate.emit({ start: startDate, end: endDate });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected readonly formatDate = formatDate;
}
