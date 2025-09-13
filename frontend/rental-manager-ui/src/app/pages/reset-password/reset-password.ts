import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss'
})
export class ResetPasswordComponent {
  token = '';
  password = '';
  confirm = '';
  loading = false;
  message = '';
  error = '';

  constructor(private route: ActivatedRoute, private router: Router, private auth: AuthService) {
    // allow token via query param or route param
    this.route.queryParamMap.subscribe(q => {
      const t = q.get('token');
      if (t) this.token = t;
    });
    this.route.paramMap.subscribe(p => {
      const t = p.get('token');
      if (t) this.token = t;
    });
  }

  submit() {
    if (this.loading) return;
    if (!this.token || !this.password || this.password !== this.confirm) {
      this.error = 'Please enter token and matching passwords.';
      return;
    }
    this.loading = true; this.error = ''; this.message = '';

    this.auth.resetPassword(this.token, this.password).subscribe({
      next: (res) => {
        this.message = res.message;
        setTimeout(() => this.router.navigateByUrl('/login'), 1200);
      },
      error: (e) => {
        this.error = e?.error?.message ?? 'Reset failed';
        this.loading = false;
      }
    });
  }
}
