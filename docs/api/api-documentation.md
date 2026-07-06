# API Documentation

The ASP.NET Core API exposes OpenAPI at `/swagger` in development.

All validation errors use RFC 7807 `ProblemDetails` or `ValidationProblemDetails`.

## Endpoint Groups

- Auth: `/api/auth/me`, `/api/auth/sync-user`
- Users: `/api/users`, `/api/users/{id}`, `/api/users/{id}/roles`, `/api/users/{id}/status`
- Donors: `/api/donors/me`, `/api/donors/{id}`
- Campaigns: `/api/campaigns`, `/api/campaigns/{slug}`, publish and update routes
- Projects: `/api/projects`, `/api/projects/{id}`
- Donations: `/api/donations`, `/api/donations/me`, `/api/donations/{id}`
- Recurring Donations: `/api/recurring-donations`, `/api/recurring-donations/{id}/cancel`
- Payments: `/api/payments/intent`, `/api/payments/webhook`
- Receipts: `/api/receipts/{id}`, `/api/receipts/{id}/resend`
- Reports: donation summary, monthly, campaign, donor, project funding, export
- Notifications: `/api/notifications`, `/api/notifications/test`
- Dashboards: donor, admin, treasurer, campaign manager

## Authorization

- Public: campaign and project read endpoints.
- Donor: donor dashboard, donation creation, receipts, profile.
- Campaign Manager: campaign and project management.
- Treasurer: reporting and donor lookup.
- Admin: user and role management.

