import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app/routes';
// Optional hydration if you use SSR:
import { provideClientHydration } from '@angular/platform-browser';
import { App } from './app/app';
import { jwtInterceptor } from './app/core/jwt-interceptor';

bootstrapApplication(App, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor])),
    provideClientHydration(), // safe to keep even without SSR
  ],
}).catch(err => console.error(err));
