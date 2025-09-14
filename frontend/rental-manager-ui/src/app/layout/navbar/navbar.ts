import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { AuthService } from '../../core/auth';


@Component({
  selector: 'app-navbar',
  standalone: true,
  // With @if/@else you don't need NgIf/AsyncPipe in imports
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html'
})
export class NavbarComponent implements OnInit, OnDestroy {
  auth = inject(AuthService);
  private router = inject(Router);

  isMenuOpen = false;
  currentUrl = '';
  sub?: Subscription;

  ngOnInit(): void {
    this.currentUrl = this.router.url;
    this.sub = this.router.events.subscribe(ev => {
      if (ev instanceof NavigationEnd) {
        this.currentUrl = ev.urlAfterRedirects;
        this.isMenuOpen = false; // close mobile menu after nav
      }
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  isAuthRoute(): boolean {
    return this.currentUrl.startsWith('/login')
        || this.currentUrl.startsWith('/signup')
        || this.currentUrl.startsWith('/reset-password');
  }

  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }
}
