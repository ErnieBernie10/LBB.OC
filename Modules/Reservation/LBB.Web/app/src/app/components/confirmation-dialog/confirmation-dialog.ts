import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Modal, ModalContent, ModalFooter, ModalHeader } from '../modal/modal';

@Component({
  selector: 'app-confirmation-dialog',
  imports: [Modal, ModalFooter, ModalContent, ModalHeader],
  templateUrl: './confirmation-dialog.html',
  styleUrl: './confirmation-dialog.scss',
})
export class ConfirmationDialog {
  @Input() title!: string;
  @Input() message!: string;
  @Input() confirmButtonText!: string;
  @Input() cancelButtonText!: string;
  @Input() loading: boolean = false;
  @Output() confirm = new EventEmitter<void>();
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() cancel = new EventEmitter<void>();
}
