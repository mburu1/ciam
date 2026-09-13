# Keycloak configuration

The local Compose stack imports [`realms/ciam-realm.json`](./realms/ciam-realm.json)
when the Keycloak data volume is initialized.

The imported realm contains:

- The `ciam-api` confidential client used by the API.
- The `ciam-spa` public client used by the Angular application.
- PKCE-enabled browser redirects for local development.
- `customer` and `administrator` realm roles.
- Email verification and password reset enabled.

Realm imports are applied only during the first initialization of a Keycloak
data store. To re-import the realm locally, remove the Compose volumes and
start the stack again:

```powershell
cd infra/docker
docker compose down -v
docker compose up --build
```

Do not run `down -v` against shared or production data.

The client secret in the realm file must match `KEYCLOAK_API_CLIENT_SECRET`
in `infra/docker/.env`. The checked-in value is for local development only;
replace it before using the stack outside a local machine.

The API uses the Keycloak service account for administrative user operations.
In a production realm, grant only the minimum `realm-management` roles needed
by that service account and manage the secret through a secret store.
