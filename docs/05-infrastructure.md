# Local infrastructure and deployment

```mermaid
flowchart TB
    Compose["Docker Compose"]
    Compose --> Postgres["postgres:17-alpine"]
    Compose --> Keycloak["keycloak:26.4"]
    Compose --> Api["Ciam.Api container"]
    Compose --> Frontend["Angular + Nginx container"]
    Compose --> Collector["OTel Collector"]
    Compose --> Prometheus["Prometheus"]
    Compose --> Grafana["Grafana"]

    Postgres -->|"ciam database"| Api
    Postgres -->|"keycloak database"| Keycloak
    Keycloak --> Api
    Frontend -->|"proxy /api and /health"| Api
    Api --> Collector
    Collector --> Prometheus
    Prometheus --> Grafana
```

## Startup ordering

```mermaid
flowchart LR
    Postgres["PostgreSQL healthy"] --> Keycloak["Keycloak healthy"]
    Postgres --> Api["API migrations + startup"]
    Keycloak --> Api
    Api --> Frontend["Frontend"]
    Collector["Collector"] --> Api
    Prometheus["Prometheus"] --> Grafana["Grafana"]
```

Compose is the local development and integration environment. A production
deployment should replace Compose volume and secret defaults with managed
PostgreSQL, a secret manager, controlled EF migration jobs, TLS, ingress, and
durable telemetry storage.
