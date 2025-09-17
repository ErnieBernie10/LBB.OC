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
import { Modal, ModalContent, ModalFooter, ModalHeader } from '../modal/modal';

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
  imports: [CommonModule, Modal, ModalHeader, ModalContent, ModalFooter],
  standalone: true,
  styleUrls: ['./scheduler.scss'],
})
export class Scheduler implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('scrollContainer') scrollContainer!: ElementRef;
  @Input() appointments: Appointment[] = [];
  @Input() weekStart?: Date;

  // Computed layout for overlapping appointments (per day)
  private layoutCache = new Map<string, Map<number, { col: number; cols: number }>>(); // dayKey -> id -> layout

  private loadingSubject = new BehaviorSubject<boolean>(false);
  private destroy$ = new Subject<void>();
  private locale = inject(LOCALE_ID);
  l = this.locale === 'nl' ? nlBE : enUS;

  @Input()
  set loading(value: boolean) {
    this.loadingSubject.next(value);
  }

  showLoading$: Observable<boolean> = withDelayedLoading(this.loadingSubject);

  // Simple modal state for selecting among overlapping sessions
  modal: { open: boolean; day: Date | null; options: Appointment[] } = { open: false, day: null, options: [] };

  @Output() appointmentCreate = new EventEmitter<{ start: Date; end: Date }>();
  @Output() appointmentUpdate = new EventEmitter<{ id: number; start: Date; end: Date }>();
  @Output() loadAppointments = new EventEmitter<{ start: Date; end: Date }>();

  currentWeek: Date = new Date();
  weekDays: Date[] = [];
  timeSlots: string[] = [];

  ngOnInit() {
    if (this.weekStart instanceof Date && !isNaN(this.weekStart.getTime())) {
      this.currentWeek = this.weekStart;
    }
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
    const dayKey = format(appointment.start, 'yyyy-MM-dd');
    const layoutForDay = this.layoutCache.get(dayKey)?.get(appointment.id);
    const startHour = appointment.start.getHours();
    const startMinute = appointment.start.getMinutes();
    const top = (startHour * 60 + startMinute) * (this.pixelsPerHour / 60);
    const duration =
      appointment.end.getHours() * 60 +
      appointment.end.getMinutes() -
      (appointment.start.getHours() * 60 + appointment.start.getMinutes());
    // We position events within the events-container, which should start at y=0 just below day-header via CSS.
    // No header offset is applied here so that the left time grid and events align pixel-perfectly.
    const gap = 2; // px gap between overlapping columns
    let left = 4; // default left padding
    let right = 4; // default right padding
    if (layoutForDay) {
      const totalCols = Math.max(1, layoutForDay.cols);
      const colIndex = layoutForDay.col;
      const columnWidth = (100 - 2) / totalCols; // percentage minus a tiny safety
      const leftPercent = colIndex * columnWidth;
      // Use percentage widths so it scales with container
      return {
        top: `${top}px`,
        height: `${duration * (this.pixelsPerHour / 60)}px`,
        left: `calc(${leftPercent}% + ${gap}px)`,
        width: `calc(${columnWidth}% - ${gap * 2}px)`,
      };
    }
    return {
      top: `${top}px`,
      height: `${duration * (this.pixelsPerHour / 60)}px`,
      left: `${left}px`,
      right: `${right}px`,
    };
  }

  getAppointmentsForDay(day: Date): Appointment[] {
    // Also compute overlap layout cache for this day when accessed
    const dayKey = format(day, 'yyyy-MM-dd');
    const dayAppointments = this.appointments
      .filter((apt) => format(apt.start, 'yyyy-MM-dd') === dayKey)
      .sort((a, b) => a.start.getTime() - b.start.getTime());

    // Build overlap groups using sweep-line and assign columns
    const layoutMap = new Map<number, { col: number; cols: number }>();
    const active: { id: number; end: number; col: number }[] = [];

    for (const apt of dayAppointments) {
      const start = apt.start.getTime();
      const end = apt.end.getTime();
      // release finished
      for (let i = active.length - 1; i >= 0; i--) {
        if (active[i].end <= start) active.splice(i, 1);
      }
      // find available column
      const usedCols = new Set(active.map((a) => a.col));
      let col = 0;
      while (usedCols.has(col)) col++;
      active.push({ id: apt.id, end, col });
      // total cols in current cluster is max of active col+1
      let cols = Math.max(...active.map((a) => a.col + 1));
      // cap visible columns to 2 in the UI; store real cols for logic using min
      const visibleCols = Math.min(2, cols);
      // update cols for all active items in this cluster
      for (const a of active) layoutMap.set(a.id, { col: Math.min(a.col, visibleCols - 1), cols: visibleCols });
    }

    this.layoutCache.set(dayKey, layoutMap);

    return dayAppointments;
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
    // Use the actual events container as reference first
    const eventsContainer = (event.currentTarget as HTMLElement) || container;
    const rectContainer = eventsContainer.getBoundingClientRect();
    const y = event.clientY - rectContainer.top + eventsContainer.scrollTop;
    const totalMinutes = Math.max(0, Math.floor((y * 60) / this.pixelsPerHour));
    const clickHour = Math.floor(totalMinutes / 60);
    const clickMinute = Math.floor((totalMinutes % 60) / 15) * 15;
    // First, check if we clicked directly on an event element
    const eventElement = (event.target as HTMLElement).closest('.event') as HTMLElement | null;
    if (eventElement) {
      // Prefer a direct mapping using a data attribute for reliability
      const idAttr = eventElement.getAttribute('data-appointment-id');
      if (idAttr) {
        const id = Number(idAttr);
        const dayAppointments = this.getAppointmentsForDay(day);
        // Determine hour window from click location
        const hourStart = clickHour * 60;
        const hourEnd = (clickHour + 1) * 60;
        // Candidates: any appointment intersecting the hour window
        const candidates = dayAppointments.filter((a) => {
          const start = a.start.getHours() * 60 + a.start.getMinutes();
          const end = a.end.getHours() * 60 + a.end.getMinutes();
          return start < hourEnd && end > hourStart; // intersects the hour window
        });
        // Build overlap graph among candidates; edge if time ranges intersect
        const overlaps = (a: Appointment, b: Appointment) => {
          const as = a.start.getHours() * 60 + a.start.getMinutes();
          const ae = a.end.getHours() * 60 + a.end.getMinutes();
          const bs = b.start.getHours() * 60 + b.start.getMinutes();
          const be = b.end.getHours() * 60 + b.end.getMinutes();
          return as < be && ae > bs;
        };
        // Find connected component that contains the clicked appointment id
        const byId = new Map<number, Appointment>(candidates.map((c) => [c.id, c]));
        const startNode = byId.get(id);
        if (startNode) {
          const visited = new Set<number>();
          const stack = [startNode];
          while (stack.length) {
            const cur = stack.pop()!;
            if (visited.has(cur.id)) continue;
            visited.add(cur.id);
            for (const next of candidates) {
              if (!visited.has(next.id) && overlaps(cur, next)) stack.push(next);
            }
          }
          const component = candidates.filter((c) => visited.has(c.id));
          if (component.length > 2) {
            // Show all overlapping blocks within the hour for this group
            this.modal = { open: true, day, options: component.sort((a, b) => a.start.getTime() - b.start.getTime()) };
            return;
          }
        }
        const existingAppointment = dayAppointments.find((a) => a.id === id);
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
    let hours = clickHour;
    let minutes = clickMinute;

    if (minutes === 60) {
      minutes = 0;
      hours += 1;
    }
    // clamp to day bounds
    if (hours < 0) hours = 0;
    if (hours > 23) hours = 23;

    const startDate = setMinutes(setHours(day, hours), minutes);
    // Default duration 15 minutes, properly roll over hour at :45
    let endMinutes = minutes + 15;
    let endHours = hours;
    if (endMinutes >= 60) {
      endMinutes = 0;
      endHours = Math.min(23, endHours + 1);
    }
    const endDate = setMinutes(setHours(day, endHours), endMinutes);
    this.appointmentCreate.emit({ start: startDate, end: endDate });
  }

  pickModalOption(opt: Appointment) {
    this.modal.open = false;
    this.appointmentUpdate.emit({ id: opt.id, start: opt.start, end: opt.end });
  }

  closeModal(ev?: Event) {
    if (ev) ev.stopPropagation();
    this.modal.open = false;
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected readonly formatDate = formatDate;
}
