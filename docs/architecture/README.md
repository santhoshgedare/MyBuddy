# Architecture foundation

MyBuddy is a modular monolith with an ASP.NET Core API and background worker.
The initial dependency direction is:

- `MyBuddy.Domain` has no project dependencies.
- `MyBuddy.Application` depends on `MyBuddy.Domain`.
- `MyBuddy.Infrastructure` implements application-facing concerns and depends
  on `MyBuddy.Application` and `MyBuddy.Domain`.
- The API and worker are composition roots; the API also references shared
  contracts.

Business modules and their persistence are intentionally deferred until this
foundation builds successfully.
