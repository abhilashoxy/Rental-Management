import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss'
})
export class ForgotPasswordComponent {
  email = '';
  loading = false;
  message = '';
  devToken: string | null = null;
  error = '';

  constructor(private auth: AuthService) {}

  submit() {
    if (this.loading || !this.email) return;
    this.loading = true; this.error = ''; this.message = ''; this.devToken = null;

    this.auth.requestPasswordReset(this.email).subscribe({
      next: (res) => {
        this.message = res.message;
        this.devToken = (res as any).devToken ?? null; // shown only in dev
        this.loading = false;
      },
      error: () => { this.error = 'Something went wrong'; this.loading = false; }
    });
  }
}
