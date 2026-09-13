# System architecture

```mermaid
flowchart LR
    Browser["Angular SPA<br/>localhost:4200"]
    Api["Ciam.Api<br/>ASP.NET Core .NET 10"]
    Keycloak["Keycloak<br/>OIDC / JWT"]
    AppDb[("PostgreSQL<br/>CIAM database")]
    KeycloakDb[("PostgreSQL<br/>Keycloak database")]
    OTel["OpenTelemetry Collector"]
    Prometheus["Prometheus"]
    Grafana["Grafana"]

    Browser -->|"REST / JSON"| Api
    Browser -->|"login / token flows"| Keycloak
    Api -->|"JWT validation and admin API"| Keycloak
    Api -->|"EF Core / Npgsql"| AppDb
    Keycloak -->|"JDBC"| KeycloakDb
    Api -.->|"OTLP traces and metrics"| OTel
    OTel --> Prometheus
    Prometheus --> Grafana
```

## Runtime responsibilities

| Component | Responsibility |
|---|---|
| Angular | UI, route protection, token attachment, profile interaction |
| Keycloak | Identity lifecycle, credentials, token issuance, verification actions |
| API | Resource authorization, application use cases, domain rules, persistence |
| PostgreSQL | Application and Keycloak persistence in separate databases |
| Collector | OTLP receiving, batching, and metrics export |
| Prometheus | Metrics storage and alert evaluation |
| Grafana | Local dashboards |
