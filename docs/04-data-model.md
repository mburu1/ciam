# Domain data model

```mermaid
erDiagram
    USER {
        uuid id PK
        string subject UK
        string email UK
        string preferred_username
        string first_name
        string last_name
        string phone_number
        string status
        boolean email_verified
        datetime created_at_utc
        datetime updated_at_utc
    }

    USER_SESSION {
        uuid id PK
        uuid user_id FK
        string refresh_token_hash
        string status
        datetime created_at_utc
        datetime expires_at_utc
        datetime revoked_at_utc
    }

    AUTHENTICATION_CHALLENGE {
        uuid id PK
        uuid user_id FK
        string purpose
        string status
        string challenge_hash
        datetime created_at_utc
        datetime expires_at_utc
        datetime completed_at_utc
    }

    USER ||--o{ USER_SESSION : owns
    USER ||--o{ AUTHENTICATION_CHALLENGE : receives
```

The `subject` value is the Keycloak identity identifier used to associate an
authenticated JWT with the local `User` aggregate. Roles, MFA methods, and
authentication methods are persisted as configured collections in the current
EF Core mapping.
