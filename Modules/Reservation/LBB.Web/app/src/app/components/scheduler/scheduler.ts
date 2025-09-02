import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { addDays, addWeeks, format, setHours, setMinutes, startOfWeek, subWeeks } from 'date-fns';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { withDelayedLoading } from '../../operators/withDelayedLoading';

export interface Appointment {
  id: string;
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

  @Input()
  set loading(value: boolean) {
    this.loadingSubject.next(value);
  }

  showLoading$: Observable<boolean> = withDelayedLoading(this.loadingSubject);

  @Output() appointmentCreate = new EventEmitter<{ start: Date; end: Date }>();
  @Output() appointmentUpdate = new EventEmitter<{ id: string; start: Date; end: Date }>();
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
    this.timeSlots = Array.from({ length: 24 }, (_, i) => format(new Date().setHours(i, 0), 'HH:mm'));
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
    const eventElement = (event.target as HTMLElement).closest('.event');
    if (eventElement) {
      // If we clicked directly on an event, find the corresponding appointment
      const appointments = this.getAppointmentsForDay(day);
      // We'll match based on position and time to find the right appointment
      const rect = eventElement.getBoundingClientRect();
      const existingAppointment = appointments.find((apt) => {
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
    const y = event.clientY - rect.top + container.scrollTop;

    const totalMinutes = Math.floor((y * 60) / this.pixelsPerHour);
    const hours = Math.floor(totalMinutes / 60);
    const minutes = Math.round((totalMinutes % 60) / 15) * 15;

    const startDate = setMinutes(setHours(day, hours - 1), minutes);
    const endDate = setMinutes(setHours(day, hours), minutes);
    this.appointmentCreate.emit({ start: startDate, end: endDate });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
