// src/app/core/jwt-interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { AuthService } from './auth';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const url = req.url || '';

  // Attach for our API calls (relative or absolute), but skip auth endpoints
  const isApi = url.startsWith('/api') || /:\/\/.+\/api\//i.test(url);
  const isAuthEndpoint = /\/Auth\/(login|register|forgot-password|reset-password)\b/i.test(url);

  let authReq = req;
  const token = auth.getToken();
  if (token && isApi && !isAuthEndpoint) {
    authReq = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }

  return next(authReq).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 || err.status === 403) {
        auth.logout();
        if (!router.url.startsWith('/login')) router.navigateByUrl('/login');
      }
      return throwError(() => err);
    })
  );
};
