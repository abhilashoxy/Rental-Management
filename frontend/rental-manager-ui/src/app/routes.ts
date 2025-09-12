import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';


export const routes: Routes = [
  // Public
  {
    path: 'login',
    loadComponent: () =>
      import('../app/pages/login/login').then(m => m.LoginComponent),
  },
  {
    path: 'signup',
    loadComponent: () =>
      import('../app/pages/signup/signup').then(m => m.SignupComponent),
  },

  // Protected
  {
    path: '',
    canMatch: [authGuard],
    loadComponent: () =>
      import('../app/pages/dashboard/dashboard').then(m => m.DashboardComponent),
  },
  {
    path: 'properties',
    canMatch: [authGuard],
    loadComponent: () =>
      import('./pages/properties/properties').then(m => m.PropertiesComponent),
  },
  {
    path: 'units',
    canMatch: [authGuard],
    loadComponent: () =>
      import('../app/pages/units/units').then(m => m.UnitsComponent),
  },
  {
    path: 'tenants',
    canMatch: [authGuard],
    loadComponent: () =>
      import('../app/pages/tenants/tenants').then(m => m.TenantsComponent),
  },

  // Fallback
  { path: '**', redirectTo: '' },
];
