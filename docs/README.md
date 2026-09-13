# CIAM documentation

This directory contains the diagrams that describe the current implementation
and the intended extension points of the CIAM platform.

## Diagrams

- [System architecture](./01-system-architecture.md)
- [Clean Architecture dependencies](./02-clean-architecture.md)
- [Authentication and registration flows](./03-authentication-flows.md)
- [Data model](./04-data-model.md)
- [Infrastructure and deployment](./05-infrastructure.md)
- [Observability pipeline](./06-observability.md)
- [Request and error flow](./07-api-request-flow.md)

Mermaid diagrams are used so they render in GitHub, VS Code Markdown preview,
and most documentation platforms without binary image assets.

The diagrams distinguish implemented behavior from planned capabilities. Items
such as WebAuthn, MFA, password reset, logout/session revocation, and custom
Keycloak SPIs remain extension points unless explicitly marked as implemented
in the source code.
