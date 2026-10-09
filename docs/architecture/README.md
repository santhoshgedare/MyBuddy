# Architecture foundation

MyBuddy is a modular monolith with an ASP.NET Core API and background worker.
The initial dependency direction is:

- `MyBuddy.Domain` has no project dependencies.
- `MyBuddy.Application` depends on `MyBuddy.Domain`.
- `MyBuddy.Infrastructure` implements application-facing concerns and depends
  on `MyBuddy.Application` and `MyBuddy.Domain`.
- The API and worker are composition roots; the API also references shared
  contracts.

Tenant-owned persistence implements `ITenantOwned` and is protected by an EF
Core global query filter. The active tenant context starts unset, which returns
no tenant-owned rows until a request has selected a tenant. The `X-Tenant-Id`
header is only a selector: API middleware requires an authenticated GUID user
identity and verifies an active membership before setting that context.

The membership lookup intentionally bypasses the global filter only for its
explicit `(userId, tenantId, active status)` predicate. Tenant-role assignment
links use composite tenant-aware foreign keys.

Business workflows, database migrations, and external identity-provider
configuration remain separate follow-up increments.
