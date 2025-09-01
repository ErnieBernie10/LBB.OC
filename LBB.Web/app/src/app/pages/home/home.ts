import { Component } from '@angular/core';

@Component({
  selector: 'app-home-page',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  modalIsOpen: boolean = false;

  toggleModal(): void {
    this.modalIsOpen = !this.modalIsOpen;
  }
}
