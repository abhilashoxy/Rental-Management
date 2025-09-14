import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, inject, Inject, PLATFORM_ID, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth';
import { NavbarComponent } from './layout/navbar/navbar';

@Component({
  selector: 'app-root',
  standalone: true,
imports: [CommonModule, RouterOutlet,NavbarComponent],

  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent {
  isBrowser = false;
  currentYear = new Date().getFullYear();
 private auth = inject(AuthService);

  constructor() {
  this.auth.rehydrate();
  }

}
