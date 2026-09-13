# CIAM – Customer Identity and Access Management

Production-oriented Customer Identity and Access Management (CIAM) platform built on **Keycloak** as the identity provider, with a **.NET 10** backend API and **Angular** frontend.

> **Core principle:** Ride an IAM tool (Keycloak) rather than building username/password handling, token issuance, session management, or credential storage from scratch.

---

## What This Project Is

This repository implements a full CIAM solution that covers:

1. **Username + Password login** via Keycloak (OIDC / JWT)  
   - Obtain access & refresh tokens  
   - Use the access token for all subsequent API calls

2. **Passwordless authentication**  
   - WebAuthn / Passkeys  
   - Designed for mobile biometrics (Fingerprint / Face ID)

3. **Security Questions**  
   - Custom Keycloak authenticator (SPI) for recovery / additional verification

4. **OpenTelemetry integration**  
   - Distributed tracing, metrics, and correlated logs across API + Keycloak

Additional production capabilities planned or scaffolded:

- Multi-factor authentication (TOTP, email OTP, WebAuthn as 2FA)
- Self-service registration, email verification, password reset
- Role / group based authorization
- Token refresh, logout, session management
- Health checks, resilience, Scalar API documentation
- Docker-based local development and CI/CD ready structure

---

## Tech Stack

| Layer              | Technology                          |
|--------------------|-------------------------------------|
| Identity Provider  | Keycloak (OIDC / OAuth 2.0)        |
| Backend            | .NET 10, ASP.NET Core, Minimal APIs / Controllers |
| API Documentation  | Scalar + Microsoft.AspNetCore.OpenApi |
| Frontend           | Angular (standalone components, SCSS) |
| Database           | PostgreSQL (Keycloak + Application data) |
| ORM                | Entity Framework Core + Npgsql      |
| Observability      | OpenTelemetry (OTLP) → Collector → Tempo / Jaeger / Grafana / Prometheus |
| Containerization   | Docker + Docker Compose             |
| CI/CD              | GitHub Actions (scaffolded)         |
| Architecture       | Clean Architecture (Api / Application / Domain / Infrastructure) |

---

## Repository Structure

```
ciam/
├── Ciam.sln / Ciam.slnx          # Solution at repository root
├── backend/
│   ├── src/
│   │   ├── Ciam.Api/             # ASP.NET Core host + Scalar + Auth + Telemetry
│   │   ├── Ciam.Application/     # Use cases, validation, interfaces
│   │   ├── Ciam.Domain/          # Entities, value objects, domain events
│   │   ├── Ciam.Infrastructure/  # EF Core, Postgres, external services
│   │   └── Ciam.Contracts/        # Shared DTOs / events
│   └── tests/                    # Unit + Integration tests
├── frontend/                     # Angular SPA
│   └── src/app/
│       ├── core/auth/            # Keycloak config, guards, interceptors
│       ├── features/auth/        # Login, passwordless, security questions
│       └── ...
├── keycloak/
│   ├── realms/                   # Realm JSON (source of truth)
│   ├── themes/                   # Custom login / account themes
│   └── providers/                # Custom SPIs (security questions)
├── infra/docker/                 # Docker Compose (local full stack)
├── observability/                # OTel Collector, Grafana, Prometheus configs
├── docs/                         # Architecture, ADRs, guides
└── scripts/                      # Helper scripts
```

---

## Key Design Decisions

- **Keycloak owns identity**  
  No custom password hashing, token issuance, or session store in the .NET application.  
  The API is a pure **resource server** that validates Keycloak-issued JWTs.

- **PostgreSQL** is the recommended (and supported) database for both Keycloak and application data.  
  Use separate databases/schemas.

- **OpenTelemetry** is first-class.  
  Both the .NET API and Keycloak export traces/metrics/logs to a central collector.

- **Clean Architecture** keeps business logic testable and independent of frameworks.

- **Angular** uses modern standalone components + feature-based folders, with Keycloak integration ready for Authorization Code + PKCE.

---

## Prerequisites

- .NET 10 SDK
- Node.js LTS + npm (for Angular)
- Docker Desktop (or equivalent) for local Keycloak + Postgres + OTel stack
- Visual Studio 2022/2026 or VS Code / Rider
- (Optional) Angular CLI

---

## Getting Started

### 1. Clone & Open

```bash
git clone <your-repo-url> ciam
cd ciam
```

Open `Ciam.sln` in Visual Studio (all projects load automatically).

### 2. Local Infrastructure (Docker)

```bash
cd infra/docker
docker compose up -d
```

This typically starts:

- Keycloak
- PostgreSQL
- OpenTelemetry Collector
- (Optional) Grafana / Tempo / Prometheus

### 3. Backend

```bash
cd backend/src/Ciam.Api
dotnet restore
dotnet run
```

- API: `https://localhost:7xxx`
- Scalar UI: `https://localhost:7xxx/scalar`

### 4. Frontend

```bash
cd frontend
npm install
npm start
```

- App: `http://localhost:4200`

### 5. Keycloak

- Admin console: `http://localhost:8080` (or configured port)
- Default admin credentials are set via environment variables in Compose
- Import the realm from `keycloak/realms/ciam-realm.json`

---

## Authentication Flows (High Level)

| Flow                    | Implementation                              |
|-------------------------|---------------------------------------------|
| Username + Password     | Keycloak Browser / Direct Grant → JWT      |
| Passwordless            | Keycloak WebAuthn Passwordless (Passkeys)  |
| Security Questions      | Custom Keycloak Authenticator (SPI)        |
| Token usage             | Angular interceptor attaches Bearer token  |
| API protection          | JWT Bearer validation + policies in .NET   |

---

## OpenTelemetry

The .NET API is instrumented with:

- `OpenTelemetry.Extensions.Hosting`
- ASP.NET Core, HttpClient, Runtime, Entity Framework Core instrumentation
- OTLP exporter

Keycloak is configured to export traces to the same collector.  
This produces end-to-end traces from the Angular client through the API into Keycloak.

---

## Development Roadmap (Suggested Order)

1. Finalize Docker Compose + Keycloak realm
2. Configure JWT authentication in `Ciam.Api`
3. Wire OpenTelemetry and verify traces
4. Implement Angular Keycloak integration (login + token interceptor)
5. Enable WebAuthn Passwordless in Keycloak + frontend support
6. Develop & deploy custom Security Questions authenticator
7. Add roles, policies, self-service flows, and hardening

---

## Contributing

- Follow Clean Architecture boundaries
- Keep Keycloak as the single source of truth for credentials and sessions
- Prefer environment variables / user secrets for configuration
- Add ADRs under `docs/adr/` for significant decisions

---

## License

[Add your license here]

---

## Authors / Maintainers

[Your name / team]
