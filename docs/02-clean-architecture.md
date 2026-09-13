# Clean Architecture dependencies

Dependencies point inward toward stable business rules. The Domain project has
no project references.

```mermaid
flowchart TB
    Api["Ciam.Api<br/>composition root"]
    Infrastructure["Ciam.Infrastructure<br/>EF Core, Keycloak, email, telemetry"]
    Application["Ciam.Application<br/>commands, queries, validators"]
    Contracts["Ciam.Contracts<br/>requests, responses, events"]
    Domain["Ciam.Domain<br/>entities, value objects, events"]

    Api --> Application
    Api --> Infrastructure
    Api --> Contracts
    Infrastructure --> Application
    Infrastructure --> Domain
    Infrastructure --> Contracts
    Application --> Domain
    Application --> Contracts
```

## Boundary rules

1. Domain code must remain framework-independent.
2. Application code depends on abstractions and coordinates use cases.
3. Infrastructure implements Application and Domain interfaces.
4. API composes dependencies and exposes HTTP endpoints.
5. Contracts are transport-facing types and must not contain persistence logic.
