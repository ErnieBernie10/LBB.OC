import { Routes } from '@angular/router';

export const routes: Routes = [
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
  {
    path: 'sessions/:id',
    loadComponent: () => import('./pages/session-detail/session-detail').then((m) => m.SessionDetailPage),
  },
  {
    path: '**',
    redirectTo: 'home',
    pathMatch: 'full',
  },
];
