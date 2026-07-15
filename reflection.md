# Reflection — improvements if more time available

This project implements a working demo of hotel search + booking with a .NET 8 API and a small Angular UI. With additional time the following improvements would be high-value:

1. Integration & E2E tests
   - Add WebApplicationFactory-based integration tests for the API to validate serialization, routing and ProblemDetails responses.
   - Add Playwright or Cypress end-to-end tests covering the full UI flow (search -> reserve -> confirm).
   - UI side unit tests

2. Stronger typing & DTO generation
   - Generate TypeScript DTOs from C# models (NSwag or custom script) to avoid manual mapping and keep client/server contracts in sync.

3. Http interceptors & centralized error handling
   - Use HttpInterceptor to handle API base URL injection, add auth placeholder, and unify ProblemDetails -> user-friendly messages.

4. Better state management for UX
   - Small store (signals or lightweight store) to keep last-search results and selected room across navigation without re-fetch.

5. Persistent data & seeded database
   - Replace InMemoryReservationStore with a simple EF Core SQLite provider for realistic persistence between runs and easier testing of concurrent reservations.

6. Accessibility and polish
   - Improve keyboard focus, ARIA attributes, and color contrast; make the responsive table fully accessible.

7. CI / CD and containerization
   - Add GitHub Actions to run dotnet build, dotnet test and npm build for the UI on PRs. Add Dockerfile(s) for local multi-container scenarios.

8. Error observability
   - Add structured logging, correlation IDs, and basic telemetry hooks (Application Insights or logging provider) for diagnosing runtime issues.

9. Config & secrets
   - Move API base URL into a runtime configuration mechanism for the Angular app (meta tag or environment injection) to support differing environments without rebuilding.

10. Performance and UX
	- Debounce search, add skeleton loading UI, and use pagination or lazy-loading if result sets grow.

11. Add Client Side Validations

These changes would move the sample from a demo to a more production-like, maintainable baseline while keeping the code intentionally simple for learning.
