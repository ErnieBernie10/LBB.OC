import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { animate, style, transition, trigger } from '@angular/animations';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.html',
  styleUrl: './modal.scss',
  animations: [
    trigger('modalAnimation', [
      transition(':enter', [style({ opacity: 0 }), animate('150ms ease-out', style({ opacity: 1 }))]),
      transition(':leave', [animate('150ms ease-in', style({ opacity: 0 }))]),
    ]),
    trigger('modalContentAnimation', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.95)' }),
        animate('150ms ease-out', style({ opacity: 1, transform: 'scale(1)' })),
      ]),
      transition(':leave', [animate('150ms ease-in', style({ opacity: 0, transform: 'scale(0.95)' }))]),
    ]),
  ],
})
export class Modal {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();

  handleOverlayClick(event: MouseEvent): void {
    // Only close if the click is directly on the dialog element
    const target = event.target as HTMLElement;
    if (target.tagName.toLowerCase() === 'dialog') {
      this.handleClose();
    }
  }

  handleClose(): void {
    this.close.emit();
  }

  @HostListener('document:keydown', ['$event'])
  documentKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      this.handleClose();
    }
  }
}

@Component({
  selector: 'modal-header',
  standalone: true,
  template: `<ng-content></ng-content>`,
})
export class ModalHeader {}

@Component({
  selector: 'modal-content',
  standalone: true,
  template: `<ng-content></ng-content>`,
})
export class ModalContent {}

@Component({
  selector: 'modal-footer',
  standalone: true,
  template: `<ng-content></ng-content>`,
})
export class ModalFooter {}
