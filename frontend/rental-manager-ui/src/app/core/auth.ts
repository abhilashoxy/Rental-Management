import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environments';

interface LoginResponse {
  token: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = environment.apiBaseUrl;
  private tokenKey = 'access_token';

  constructor(private http: HttpClient) {}

  // 🔐 Login
  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.base}/api/auth/login`, {
      email,
      password,
    });
  }

  // 🆕 Register
  register(email: string, password: string, role: string) {
    return this.http.post(`${this.base}/api/auth/register`, {
      email,
      password,
      role,
    });
  }

  // 💾 Save token
  saveToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
  }

  // 📤 Logout
  logout() {
    localStorage.removeItem(this.tokenKey);
  }

  // ✅ Is user logged in
  isAuthenticated(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }

  // 📦 Get current token
  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}
