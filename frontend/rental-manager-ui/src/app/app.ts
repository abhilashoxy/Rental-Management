import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth';

@Component({
  selector: 'app-root',
  standalone: true,
imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],

  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent {
  isBrowser = false;
  constructor(public auth: AuthService, @Inject(PLATFORM_ID) platformId: Object) {
    this.isBrowser = isPlatformBrowser(platformId);
  }
  logout() { this.auth.logout(); }
  collapseNav() { /* (keep your collapse code here if you added it) */ }
}
