import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideClientHydration } from '@angular/platform-browser';
import 'zone.js'; // ✅ required for Angular’s default change detection
import { routes } from './app/routes';
import { AppComponent } from './app/app';
import { jwtInterceptor } from './app/core/jwt-interceptor';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor])),
    provideClientHydration(), // ok even without SSR
  ],
}).catch(console.error);
