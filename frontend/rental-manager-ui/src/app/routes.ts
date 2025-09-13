import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password';
import { ResetPasswordComponent } from './pages/reset-password/reset-password';


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
  { path: 'forgot-password',
     loadComponent: () =>
      import('../app/pages/forgot-password/forgot-password').then(m => m.ForgotPasswordComponent),
    },
  // accept both /reset-password and /reset-password/:token
  { path: 'reset-password',
     loadComponent: () =>
      import('../app/pages/reset-password/reset-password').then(m => m.ResetPasswordComponent),
    },

  {path: 'reset-password/:token', loadComponent: () =>
      import('../app/pages/reset-password/reset-password').then(m => m.ResetPasswordComponent),
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
  { path: 'profile',
    canMatch: [authGuard],
    loadComponent: () =>
      import('../app/pages/profile/profile').then(m => m.ProfileComponent),
  },
  { path: 'profile/edit', canActivate: [authGuard], loadComponent: () => import('../app/pages/profile-edit/profile-edit').then(m => m.ProfileEditComponent) },

  // Fallback
  { path: '**', redirectTo: '' },
];
