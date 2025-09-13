import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService, AuthUser } from '../../core/auth';



@Component({
  selector: 'app-profile-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile-edit.html',
})
export class ProfileEditComponent implements OnInit, OnDestroy {
  form!: FormGroup;
  pwdForm!: FormGroup;

  saving = false;
  changing = false;
  error = '';
  message = '';
  private sub?: Subscription;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    // initialize here
    this.form = this.fb.group({
      name: ['', [Validators.maxLength(80)]],
    });

    this.pwdForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
    });

    // populate form from user stream
    this.sub = this.auth.user$.subscribe((u: AuthUser | null) => {
      if (u) this.form.patchValue({ name: u.name ?? '' });
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  save(): void {
    if (this.form.invalid) return;
    this.saving = true;
    this.error = this.message = '';

    this.auth.updateProfile({ name: this.form.value.name ?? '' }).subscribe({
      next: () => {
        this.message = 'Profile updated.';
        this.saving = false;
        setTimeout(() => this.router.navigateByUrl('/profile'), 300);
      },
      error: (e) => {
        this.error = e?.error ?? 'Failed to update profile.';
        this.saving = false;
      }
    });
  }



  back(): void {
    this.router.navigateByUrl('/profile');
  }
}
