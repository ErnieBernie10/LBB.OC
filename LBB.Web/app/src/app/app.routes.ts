import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
  },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home').then((m) => m.Home),
  },
  {
    path: 'scheduler',
    loadComponent: () => import('./pages/scheduler/scheduler').then((m) => m.Scheduler),
  },
  {
    path: 'reservations',
    loadComponent: () => import('./pages/reservations/reservations').then((m) => m.Reservations),
  },
];
