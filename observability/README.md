# Observability

The local stack collects OpenTelemetry metrics and traces from the API.

## Components

- OpenTelemetry Collector receives OTLP over gRPC (`4317`) and HTTP (`4318`).
- Prometheus stores metrics exposed by the Collector on `8889`.
- Grafana is provisioned with Prometheus and a CIAM overview dashboard.

Local URLs:

- Prometheus: `http://localhost:9090`
- Grafana: `http://localhost:3000`

The current local trace pipeline writes traces to the Collector debug exporter.
Production deployments should replace that exporter with a durable backend such
as Tempo or Jaeger and configure retention, access control, and storage.

The provided credentials are development defaults. Set
`GRAFANA_ADMIN_PASSWORD` in `infra/docker/.env` before exposing Grafana beyond
the local machine.
