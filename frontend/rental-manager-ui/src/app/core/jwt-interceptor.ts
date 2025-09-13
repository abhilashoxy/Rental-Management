// src/app/core/jwt-interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthService } from './auth';

const AUTH_SKIP = [
  '/auth/login',
  '/auth/register',
  '/auth/forgot-password',
  '/auth/reset-password',
  '/Auth/login',            // keep your backend's casing just in case
  '/Auth/register',
  '/Auth/forgot-password',
  '/Auth/reset-password',
];

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // Don’t attach to non-http(s) or assets
  const url = req.url || '';
  const isHttp = url.startsWith('http://') || url.startsWith('https://');
  const shouldSkip = AUTH_SKIP.some(p => url.includes(p)) || !isHttp;

  const token = auth.getToken();

  const authReq = (!shouldSkip && token)
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 || err.status === 403) {
        // Token missing/expired/invalid → clear and go to login
        auth.logout();
        // Avoid redirect loop if already on /login
        if (!router.url.startsWith('/login')) {
          router.navigateByUrl('/login');
        }
      }
      return throwError(() => err);
    })
  );
};
