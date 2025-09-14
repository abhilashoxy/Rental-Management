import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

import { AuthService, AuthUser } from '../../core/auth';




import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.html',
  standalone: true,
  imports: [CommonModule]
})
export class ProfileComponent implements OnInit {
goToLogin(e: Event) {
e.preventDefault();
  this.router.navigateByUrl('/login');
}

goProfileEdit(e: Event) {
e.preventDefault();
  this.router.navigateByUrl('/profile/edit');
}
  user: AuthUser | null = null;

  constructor(private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    const token = this.auth.getToken();
    if (token) {
      try {
        const decoded = this.decodeToken(token) as any;
        this.user = {
          email: decoded.sub,
          role: decoded.role??null,
          //role: decoded.$`http://schemas.microsoft.com/ws/2008/06/identity/claims/role`,
          name: decoded.name ?? decoded.sub.split('@')[0],
          token:token
        };
      } catch(Ex) {
        this.user = null;
      }
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
  private decodeToken(token: string): any {
  try {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
  } catch {
    return null;
  }
}

}
