# Authentication and registration flows

## Registration

```mermaid
sequenceDiagram
    autonumber
    participant User as Browser
    participant Api as Ciam.Api
    participant App as Application
    participant IdP as Keycloak
    participant Db as CIAM PostgreSQL

    User->>Api: POST /api/auth/register
    Api->>App: RegisterUserCommand
    App->>App: Validate request
    App->>Db: Check email uniqueness
    App->>IdP: Create identity user
    IdP-->>App: Keycloak subject
    App->>Db: Create local User aggregate
    App->>Db: Save changes
    App-->>Api: UserProfileResponse
    Api-->>User: 200 OK
```

## Login and protected API access

```mermaid
sequenceDiagram
    autonumber
    participant User as Browser
    participant IdP as Keycloak
    participant Api as Ciam.Api
    participant Db as CIAM PostgreSQL

    User->>IdP: Authenticate with credentials
    IdP-->>User: Access token + refresh token
    User->>Api: Request with Bearer access token
    Api->>IdP: Discover issuer/signing keys
    Api->>Api: Validate issuer, audience, signature, lifetime
    Api->>Db: Load current user by subject
    Db-->>Api: User aggregate
    Api-->>User: Protected response
```

The current API uses Keycloak-issued JWTs as a resource server. Passwordless
WebAuthn, MFA, password reset, and logout/session revocation are planned
extension points and are not represented as completed application flows here.
