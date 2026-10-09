# Security foundation

- MyBuddy Identity uses GUID user IDs, unique email addresses, a 12-character
  password minimum, complexity requirements, and account lockout after five
  failed attempts.
- API routes require an authenticated identity and a valid `X-Tenant-Id`.
  The requested tenant is accepted only after the identity has an active
  membership; the header alone never grants access.
- Tenant-owned EF Core queries fail closed until the authorized tenant context
  is set. Membership resolution bypasses those filters only for an exact
  user-and-tenant membership check.
- Identity cookies are HttpOnly, Secure-only, and use SameSite=Lax. API
  authorization failures return status codes instead of redirecting to HTML.
- Supply production database credentials and Identity/Data Protection key-ring
  storage through deployment secret/configuration providers. Do not commit
  credentials. Production key rings must be durable and protected by the
  deployment's encryption/key-management service.

Local login endpoints, Entra ID OIDC configuration, CSRF protections for
state-changing cookie-authenticated endpoints, invitations, and authorization
policies are not yet implemented. Do not expose state-changing cookie-auth
routes until the corresponding antiforgery protections are added.
