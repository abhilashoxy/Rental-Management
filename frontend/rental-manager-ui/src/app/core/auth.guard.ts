import { CanMatchFn, Router, UrlTree } from '@angular/router';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export const authGuard: CanMatchFn = (route, segments): boolean | UrlTree => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);

  // Only read localStorage in the browser
  const token = isPlatformBrowser(platformId)
    ? globalThis.localStorage?.getItem('access_token')
    : null;

  if (token) return true;

  const returnUrl = '/' + segments.map(s => s.path).join('/');
  return router.createUrlTree(['/login'], { queryParams: { returnUrl } });
};
