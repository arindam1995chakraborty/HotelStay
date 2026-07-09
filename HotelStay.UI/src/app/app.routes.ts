import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', loadComponent: () => import('./search/search.component').then(m => m.SearchComponent) },
  { path: 'results', loadComponent: () => import('./results/results.component').then(m => m.ResultsComponent) },
  { path: 'reserve', loadComponent: () => import('./reservation/reservation.component').then(m => m.ReservationComponent) },
  { path: 'confirmation/:reference', loadComponent: () => import('./confirmation/confirmation.component').then(m => m.ConfirmationComponent) }
];
