# API request and error flow

```mermaid
flowchart TD
    Request["HTTP request"] --> Forwarded["Forwarded headers"]
    Forwarded --> Logging["Serilog request logging"]
    Logging --> Cors["CORS policy"]
    Cors --> Auth["JWT authentication"]
    Auth --> Authorization["Authorization policy"]
    Authorization --> RateLimit["Rate limiter"]
    RateLimit --> Endpoint["Minimal API endpoint"]
    Endpoint --> Mediator["MediatR command or query"]
    Mediator --> Validation["FluentValidation behavior"]
    Validation --> Handler["Application handler"]
    Handler --> Repository["Repository / external service"]
    Repository --> Response["HTTP response"]

    Middleware["ExceptionHandlingMiddleware"] -.->|"maps unhandled exceptions"| Error["API error response"]
    Logging -.->|"critical failures"| ErrorLog["Serilog critical log file"]
    Error --> ErrorLog
```

## Health endpoints

```mermaid
flowchart LR
    Live["GET /health/live"] --> LiveResult["Process liveness only"]
    Ready["GET /health/ready"] --> Postgres["PostgreSQL check"]
    Ready --> Keycloak["Keycloak realm check"]
    Postgres --> ReadyResult["Readiness result"]
    Keycloak --> ReadyResult
```

`/health/live` is suitable for process liveness probes. `/health/ready`
includes the dependencies required for the API to serve authenticated traffic.
