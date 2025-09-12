// src/app/app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors, withFetch } from '@angular/common/http';
import { provideClientHydration } from '@angular/platform-browser';
import { provideServerRendering } from '@angular/platform-server';

import { routes } from './routes';
import { jwtInterceptor } from './core/jwt-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withFetch(),                     // 👈 use fetch on the server
      withInterceptors([jwtInterceptor])
    ),
    provideClientHydration(),
    provideServerRendering(),
  ],
};
