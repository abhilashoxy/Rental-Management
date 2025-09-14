// src/app/core/auth.service.ts
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable, map, tap } from 'rxjs';

import { StorageService } from './storage';
import { environment } from '../../environments/environments';

// keep in sync with your backend JWT claims
export interface AuthUser {
  email: string;
  role: string;
  name?: string;
  token: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private isBrowser: boolean;

  // in-memory auth state
  public _user$ = new BehaviorSubject<AuthUser | null>(null);
  public user$ = this._user$.asObservable();
  public isAuthenticated$ = this.user$.pipe(map(u => !!u));

  constructor(
    private http: HttpClient,
    private storage: StorageService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(this.platformId);
    // Initialize auth state after isBrowser is set
    this._user$.next(this.readUserFromStorage());
  }

  // ---------- Public API ----------

  login(email: string, password: string): Observable<AuthUser> {
    return this.http.post<{ token: string; email?: string; role?: string }>(
      `${environment.apiBaseUrl}/Auth/login`,
      { email, password }
    ).pipe(
      map(res => this.buildUserFromToken(res.token, email, res.role)),
      tap(user => this.setUser(user))
    );
  }

  register(email: string, password: string, role: string): Observable<any> {
    return this.http.post(`${environment.apiBaseUrl}/Auth/register`, { email, password, role });
  }

  logout(): void {
    this._user$.next(null);
    if (this.isBrowser) {
      this.storage.removeItem('access_token');
      this.storage.removeItem('auth_user');
    }
  }

  /** Immediate boolean (use isAuthenticated$ in templates for reactivity) */
  isAuthenticated(): boolean {
    return !!this._user$.value;
  }

  /** Raw JWT for interceptors if you still read it there */
  getToken(): string | null {
    return this._user$.value?.token ?? (this.isBrowser ? this.storage.getItem('access_token') : null);
  }

  requestPasswordReset(email: string) {
    // keep Auth casing to match your backend controller
    return this.http.post<{ message: string; devToken?: string }>(
      `${environment.apiBaseUrl}/Auth/forgot-password`,
      { email }
    );
  }

  resetPassword(token: string, newPassword: string) {
    return this.http.post<{ message: string }>(
      `${environment.apiBaseUrl}/Auth/reset-password`,
      { token, newPassword }
    );
  }

  // ---------- Internals ----------

  private setUser(user: AuthUser | null) {
    this._user$.next(user);
    if (!this.isBrowser) return;
    if (user) {
      this.storage.setItem('access_token', user.token);
      this.storage.setItem('auth_user', JSON.stringify(user));
    } else {
      this.storage.removeItem('access_token');
      this.storage.removeItem('auth_user');
    }
  }

  private readUserFromStorage(): AuthUser | null {
    if (!this.isBrowser) return null;
    const token = this.storage.getItem('access_token');
    const rawUser = this.storage.getItem('auth_user');
    if (rawUser) {
      try { return JSON.parse(rawUser) as AuthUser; } catch { /* fall through */ }
    }
    if (token) return this.buildUserFromToken(token);
    return null;
  }

  private buildUserFromToken(token: string, fallbackEmail?: string, fallbackRole?: string): AuthUser {
    const claims = this.decodeJwt(token);
    const email = claims['email'] || fallbackEmail || '';
    const role = claims['role'] ||
      claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
       fallbackRole || '';
    const name =
      claims['name'] || claims['given_name'] || claims['unique_name'] || email;

    return { email, role, name, token };
  }

  private decodeJwt(token: string): Record<string, any> {
    try {
      const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
      const json = typeof atob !== 'undefined'
        ? atob(base64)
        : Buffer.from(base64, 'base64').toString('binary');
      // decodeURIComponent(escape(...)) handles unicode
      return JSON.parse(decodeURIComponent(escape(json)));
    } catch {
      return {};
    }
  }
  rehydrate(): void {
  const user = this.readUserFromStorage();
  // use the internal setter so it also writes/removes storage consistently
  (this as any).setUser(user);
}
updateProfile(payload: { name?: string }) {
  return this.http.put<{ user: AuthUser; newJwt?: string }>(
    `${environment.apiBaseUrl}/Auth/me`,
    payload
  ).pipe(
    tap(res => {
      if (res.newJwt) {
        // rebuild user from new token so navbar/profile update
        const updated = this.buildUserFromToken(res.newJwt);
        this.setUser(updated);
      } else if (res.user) {
        // keep existing token, just patch name locally
        const cur = this._user$.value;
        if (cur) this.setUser({ ...cur, name: res.user.name ?? cur.name });
      }
    })
  );
}

getMe() {
  return this.http.get<{ id: number; email: string; name?: string; role: string }>(
    `${environment.apiBaseUrl}/Users/me`
  );
}
}
