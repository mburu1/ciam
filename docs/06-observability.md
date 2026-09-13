# Observability pipeline

```mermaid
flowchart LR
    Api["Ciam.Api"]
    Api -->|"OTLP gRPC :4317<br/>or HTTP :4318"| Collector["OTel Collector"]
    Collector -->|"batch"| Metrics["Prometheus exporter :8889"]
    Metrics --> Prometheus["Prometheus"]
    Prometheus -->|"queries and alerts"| Grafana["Grafana"]
    Collector -->|"debug exporter<br/>local traces"| Logs["Collector output"]
```

## Signals

| Signal | Source | Current destination |
|---|---|---|
| Metrics | ASP.NET Core, HttpClient, runtime | Collector, Prometheus, Grafana |
| Traces | ASP.NET Core, HttpClient, custom activity source | Collector debug exporter |
| Logs | Serilog API logging | Console and rolling critical log files |

The local trace exporter is intentionally a debug exporter. Production should
add a durable trace backend such as Grafana Tempo or Jaeger before relying on
historical trace analysis.
