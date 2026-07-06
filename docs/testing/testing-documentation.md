# Testing Documentation

## Backend

- Unit test domain rules and validators.
- Integration test API endpoints using `WebApplicationFactory`.
- Verify RBAC policies for every protected endpoint.
- Verify `ProblemDetails` response shape for validation and auth failures.
- Verify payment webhook signature handling once the real provider is selected.

## Frontend

- Component test forms, campaign cards, dashboard stats, report widgets.
- E2E test donation flow, dashboard navigation, admin campaign creation, role management, and report export.

## Infrastructure

- Run `az bicep build --file infra/bicep/main.bicep`.
- Run `az deployment group what-if` before deployment.
- Smoke test `/health`, frontend routes, login redirect, and Application Insights telemetry.

