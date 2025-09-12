# Copilot Instructions for AI Agents

## Project Overview
- This is an Angular 17+ single-page application for rental property management.
- The main entry point is `src/main.ts`, which bootstraps the app with `AppComponent` and sets up routing, HTTP client (with JWT interceptor), and client hydration.
- App structure is modular: features are organized under `src/app/pages/` (e.g., `dashboard`, `login`, `properties`, `tenants`, `units`).
- Core logic (API, authentication, interceptors) is in `src/app/core/`.
- Data models are in `src/app/models/`.
- Routing is defined in `src/app/routes.ts` and feature-specific route files.

## Key Patterns & Conventions
- Use Angular's standalone component and routing APIs (no `NgModule` files).
- HTTP requests are intercepted for JWT auth via `jwtInterceptor` (`src/app/core/jwt-interceptor.ts`).
- Services and guards are provided at the root or feature level, not via modules.
- Styles are SCSS, with global styles in `src/styles.scss` and per-feature styles in each page/component folder.
- Use Angular CLI for scaffolding and builds; avoid custom scripts unless necessary.

## Developer Workflows
- **Start dev server:** `ng serve` (or `npm start`)
- **Run unit tests:** `ng test` (or `npm test`)
- **Build for production:** `ng build`
- **Scaffold components:** `ng generate component <name>`
- **Debug:** Use browser dev tools; source maps are enabled by default in dev mode.

## Integration & Extensibility
- All HTTP API calls should use Angular's `HttpClient` and respect the JWT interceptor.
- Add new features as folders under `src/app/pages/` and register routes in the appropriate route file.
- For cross-cutting concerns (e.g., auth, API), extend or add to `src/app/core/`.

## Examples
- To add a new page:
  1. Scaffold with `ng generate component pages/newpage`
  2. Add route in `src/app/routes.ts`
  3. Implement logic in the new folder
- To add a new model: create a TypeScript file in `src/app/models/` and export it via `index.ts`.

## References
- See `README.md` for CLI usage and more details.
- Key files: `src/main.ts`, `src/app/app.ts`, `src/app/routes.ts`, `src/app/core/`, `src/app/models/`, `src/app/pages/`

---

If unsure about a pattern, prefer Angular CLI conventions and check for similar patterns in existing `src/app/pages/` or `src/app/core/` files.
