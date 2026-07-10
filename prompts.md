# AI prompts used

This file records the key prompts and intent used while developing the project. It is a compact log for reproducibility and rationale.

1) "Create an Angular app that integrates with the HotelStay.API endpoints: search (GET), reserve (POST), confirmation (GET). Use environment.ts, prefer query params, no extra UI framework." 
- Decision: Use Angular standalone components, reactive forms, and query params to keep pages bookmarkable.

2) "Implement hotel-api.service to call backend, normalize server PascalCase responses to camelCase used in TS client." 
- Decision: Add mapping layer in the service to avoid duplicating mapping logic in components.

3) "Fix CORS error in API" 
- Decision: Add a named CORS policy allowing common local dev origins (4200, 5173). Keep policy targeted to localhost for safety.

4) "Fix loader/stale UI issues when observable doesn't complete" 
- Decision: Clear loading flag in next/error handlers and use explicit unsubscribe to cancel in-flight requests.

5) "Unit tests for HotelService and HotelsController with XUnit" 
- Decision: Add tests that exercise positive and negative scenarios with real provider stubs and in-memory store for deterministic behavior.

6) "Improve UI styling, keep simple SCSS per component" 
- Decision: Add small component-scoped SCSS files and a minimal app shell instead of external frameworks.

Notes on prompt structure
------------------------
- Prompts prioritized clarity about API contracts, error handling, and developer ergonomics (local dev ports, environment.ts).
- When choosing implementation details (e.g., toPromise -> firstValueFrom vs subscription), preference was given to explicit, easy-to-review patterns.
