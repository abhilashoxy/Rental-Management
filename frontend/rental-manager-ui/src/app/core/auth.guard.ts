import { CanMatchFn, Router } from '@angular/router';

export const authGuard: CanMatchFn = (route, segments) => {
  const token = typeof localStorage !== 'undefined' ? localStorage.getItem('access_token') : null;

  // allow if token exists
  if (token) return true;

  // redirect to /login preserving target url
  const router = new Router();
  router.navigate(['/login'], {
    queryParams: { returnUrl: '/' + segments.map(s => s.path).join('/') }
  });
  return false;
};
