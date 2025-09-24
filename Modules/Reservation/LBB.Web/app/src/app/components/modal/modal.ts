import { Component, Input, Output, EventEmitter, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.html',
  styleUrl: './modal.scss',
})
export class Modal {
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();

  handleOverlayClick(event: MouseEvent): void {
    // Only close if clicked on the <dialog> itself (the backdrop)
    if (event.target === event.currentTarget) {
      this.handleClose();
    }
  }

  handleClose(): void {
    this.isOpen = false;
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
