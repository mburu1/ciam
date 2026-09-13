#!/bin/bash
set -euo pipefail

app_user="${APP_DB_USER:-ciam}"
app_password="${APP_DB_PASSWORD:-local-ciam-db-password}"
keycloak_user="${KEYCLOAK_DB_USER:-keycloak}"
keycloak_password="${KEYCLOAK_DB_PASSWORD:-local-keycloak-db-password}"

psql -v ON_ERROR_STOP=1 \
  -v app_user="$app_user" \
  -v app_password="$app_password" \
  -v keycloak_user="$keycloak_user" \
  -v keycloak_password="$keycloak_password" \
  --username "$POSTGRES_USER" --dbname postgres <<-EOSQL
CREATE USER :"app_user" WITH PASSWORD :'app_password';
CREATE DATABASE ciam OWNER :"app_user";
CREATE USER :"keycloak_user" WITH PASSWORD :'keycloak_password';
CREATE DATABASE keycloak OWNER :"keycloak_user";
EOSQL
