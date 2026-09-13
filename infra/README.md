# Local infrastructure

## Start the full stack

1. Copy `infra/docker/.env.example` to `infra/docker/.env`.
2. Replace all development passwords before sharing the environment.
3. Start the stack:

```powershell
cd infra/docker
docker compose up --build
```

Services:

- Frontend: `http://localhost:4200`
- API: `http://localhost:5240`
- Keycloak: `http://localhost:8080`
- Grafana: `http://localhost:3000`
- Prometheus: `http://localhost:9090`

The first PostgreSQL initialization creates separate `ciam` and `keycloak`
databases. Initialization scripts run only when the `postgres-data` volume is
created for the first time. To recreate local databases:

```powershell
docker compose down -v
docker compose up --build
```

Do not use `down -v` against shared or production data.

The API applies EF migrations in the local Compose environment through
`DATABASE_APPLY_MIGRATIONS=true`. Production deployments should run migrations
as a controlled release step and set this to `false`.
