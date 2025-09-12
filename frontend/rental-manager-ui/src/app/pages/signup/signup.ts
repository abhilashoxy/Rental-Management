import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './signup.html',
  styleUrl: './signup.scss'
})
export class SignupComponent {
  firstName = '';
  lastName = '';
  email = '';
  password = '';
  role: 'Admin' | 'Manager' | 'Viewer' = 'Admin';
  loading = false;
  error = '';
  success = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit() {
    if (!this.email || !this.password) return;
    this.loading = true;
    this.error = '';
    this.success = '';

    // Backend expects: { email, password, role }
    this.auth.register(this.email, this.password, this.role).subscribe({
      next: () => {
        this.success = 'Account created. Redirecting to login...';
        setTimeout(() => this.router.navigateByUrl('/login'), 900);
      },
      error: (e) => {
        this.error =
          (e?.error && (e.error.message || e.error)) ||
          'Failed to create account';
        this.loading = false;
      },
    });
  }
}

