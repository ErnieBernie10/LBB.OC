import { Component, inject, OnInit } from '@angular/core';
import { PlatformLocation } from '@angular/common';

@Component({
  selector: 'app-home-page',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  platformLocation = inject(PlatformLocation);
  ngOnInit(): void {
    fetch(this.platformLocation.getBaseHrefFromDOM() + 'sessions')
      .then((res) => res.json())
      .then((json) => {
        console.log(json);
      })
      .catch((error) => {
        console.log(error);
      });
  }
  modalIsOpen: boolean = false;

  toggleModal(): void {
    this.modalIsOpen = !this.modalIsOpen;
  }
}
