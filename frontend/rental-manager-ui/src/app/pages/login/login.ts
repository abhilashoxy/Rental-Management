import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/auth';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
   standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  email = '';
  password = '';
  loading = false;
  error = '';

  constructor(private auth: AuthService, private router: Router,private route: ActivatedRoute) {}

  onSubmit() {
    if (!this.email || !this.password) return;
    this.loading = true;
    this.error = '';
    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
         this.auth.rehydrate(); 
        if (res?.token) {
      this.auth.logout(); // clear old token
      this.auth['storage'].setItem('access_token', res.token);
    }
     const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') || '/';
    this.router.navigateByUrl(returnUrl);
      },
      error: (err: HttpErrorResponse) => {
        // Try to read error message from backend, fallback to generic
        const msg =
          (err?.error && (err.error.message || err.error.Error || err.error.error)) ||
          err.statusText ||
          'Invalid credentials';
        this.error = String(msg);
        this.loading = false;
      },
    });
  }
  goSignup(e: Event) {
  e.preventDefault();
  this.router.navigateByUrl('/signup');
}
goForgotPassword(e: Event) {
  e.preventDefault();
  this.router.navigateByUrl('/forgot-password');
}
}
