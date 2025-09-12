import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';

import { Observable } from 'rxjs';
import { StorageService } from './storage';
import { environment } from '../../environments/environments';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(
    private http: HttpClient,
    private storage: StorageService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  login(email: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${environment.apiBaseUrl}/Auth/login`, { email, password });
  }

  // ✅ add this
  register(email: string, password: string, role: string): Observable<any> {
    // adjust the endpoint to match your backend (e.g. /api/Auth/register or /api/Users/register)
    return this.http.post(`${environment.apiBaseUrl}/Auth/register`, { email, password, role });
  }

  saveToken(token: string) { this.storage.setItem('access_token', token); }
  logout() { this.storage.removeItem('access_token'); }

  isAuthenticated(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;
    return !!this.storage.getItem('access_token');
  }

  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return this.storage.getItem('access_token');
  }
}
