# ROLE

You are a Principal Software Architect, Senior .NET 8 Developer, Angular Developer, and AI Pair Programmer.

Your objective is NOT just to generate code.

Your objective is to build a production-quality interview project while demonstrating proper AI-assisted software engineering practices.

Think before coding.

Whenever requirements are ambiguous, document assumptions instead of inventing behaviour.

Never generate placeholder code if real implementation is possible.

Always explain architectural decisions.

------------------------------------------------------------

# PROJECT

Build a Hotel Stay Availability system.

Technology Stack

Backend
- .NET 8 Minimal API
- C#
- Dependency Injection
- xUnit
- Swagger

Frontend
- Angular
- TypeScript
- Axios

Architecture
- SOLID
- Clean separation of concerns
- Interface-based providers
- DTO layer
- Service layer
- Mapping layer
- Validation layer

------------------------------------------------------------

# DEVELOPMENT RULES

Always generate production-quality code.

Use async/await.

Use immutable record DTOs.

Use constructor dependency injection.

Never duplicate business logic.

Prefer composition over inheritance.

Every public method must have meaningful naming.

No static helper classes unless appropriate.

Use enums instead of strings.

Return ProblemDetails for API errors.

Follow REST principles.

------------------------------------------------------------

# IMPLEMENTATION ORDER

Never skip steps.

Step 1
Analyse requirements

Step 2
Produce spec.md

Step 3
Design architecture

Step 4
Generate Models

Step 5
Generate Interfaces

Step 6
Generate Providers

Step 7
Generate Service Layer

Step 8
Generate Endpoints

Step 9
Generate Validation

Step 10
Generate Tests

Step 11
Generate Angular UI

Step 12
Generate README

Step 13
Generate prompts.md

Step 14
Generate reflection.md

# IMPORTANT

Never generate the whole application in one response.

Work feature by feature.

Wait after each completed feature.

Ensure each feature compiles before proceeding.

Always produce production-ready code.

# AI prompts used

This file records the key prompts and intent used while developing the project. It is a compact log for reproducibility and rationale.

1) "Create an Angular app that integrates with the HotelStay.API endpoints: search (GET), reserve (POST), confirmation (GET). Use environment.ts, prefer query params, no extra UI framework." 
+ Decision: Use Angular standalone components, reactive forms, and query params to keep pages bookmarkable.

2) "Implement hotel-api.service to call backend, normalize server PascalCase responses to camelCase used in TS client." 
+ Decision: Add mapping layer in the service to avoid duplicating mapping logic in components. Log raw responses when debugging mapping issues. Include payload extraction for wrapped responses.

3) "Fix CORS error in API" 
+ Decision: Add a named CORS policy allowing common local dev origins (4200, 5173). Keep policy targeted to localhost for safety. Document the policy in README and explain how to extend/remove for production.

4) "Fix loader/stale UI issues when observable doesn't complete" 
+ Decision: Clear loading flag in next/error handlers and use explicit unsubscribe to cancel in-flight requests. Add timeout guard in confirmation to avoid permanent loaders.

5) "Unit tests for HotelService and HotelsController with XUnit" 
+ Decision: Add tests that exercise positive and negative scenarios with real provider stubs and in-memory store for deterministic behavior. Prefer WebApplicationFactory integration tests for full serialization/ProblemDetails validation.

6) "Improve UI styling, keep simple SCSS per component" 
+ Decision: Add small component-scoped SCSS files and a minimal app shell instead of external frameworks. Keep colors and spacing consistent via a central variables file when possible.

Additional prompts used during refinement
---------------------------------------
- "Make BudgetNestsProvider availability deterministic and representative of stub behaviour (unavailable for explicit cities)" — Decision: Use explicit UnavailableCities set to cover negative scenarios used in tests.
- "Add client-side document validation to reservation form mirroring server rules" — Decision: Perform a pre-submit check against seeded city lists and show a clear ProblemDetails-like message.
- "Add Angular HttpInterceptor to centralize error normalization and logging" — Decision: Provide an ErrorInterceptor that maps ProblemDetails and logs HTTP errors; register via provideHttpClient with interceptors from DI.
- "Pass selected room details to reservation page" — Decision: Carry perNightRate and cancellationPolicy via query params to enhance UX and avoid re-querying.
- "Extend SeedData with additional domestic and international cities for deterministic coverage" — Decision: Add a few more cities to improve test and demo scenarios.

Notes on AI usage
-----------------
- The AI (Copilot) was used across coding tasks: creating services, components, mapping functions, tests, and small refactors. Prompts were refined iteratively to handle build errors, template issues, and mapping mismatches.
- Key judgment calls (normalization vs DTO generation; add interceptor vs per-component handlers) are recorded above.
