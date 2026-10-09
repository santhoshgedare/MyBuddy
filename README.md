# MyBuddy

MyBuddy is a multi-tenant employee and project management platform. This
repository is being built incrementally; the current foundation contains the
.NET 10 solution and an Angular application shell.

## Repository baseline

The initial repository contained only this README and no application code,
build configuration, or tests. The first implementation increment establishes
the solution structure and build conventions before business modules are added.

## Build and test

```sh
dotnet build
dotnet test
```

For the web client:

```sh
cd src/Clients/mybuddy-web
npm ci
npm run build
npm test -- --watch=false
```

The backend targets .NET 10. Project dependencies are centrally versioned in
`Directory.Packages.props`. The domain project has no infrastructure reference;
application logic depends on the domain, while infrastructure implements
application-facing concerns.
