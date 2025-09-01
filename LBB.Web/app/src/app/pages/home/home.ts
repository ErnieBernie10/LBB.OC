import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-home-page',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit {
  ngOnInit(): void {
    fetch(environment.apiUrl + '/sessions')
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
