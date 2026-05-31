#!/bin/bash
set -e

DB_HOST="${DB_SERVER%%,*}"
DB_PORT="${DB_SERVER##*,}"
[ "$DB_PORT" = "$DB_SERVER" ] && DB_PORT=1433
DB_HOST="${DB_HOST:-localhost}"

MAX_RETRIES=${DB_WAIT_RETRIES:-60}
RETRY_INTERVAL=2

echo "Waiting for database at ${DB_HOST}:${DB_PORT}..."
count=0
until command -v bash &>/dev/null && bash -c "exec 3<>/dev/tcp/${DB_HOST}/${DB_PORT}" 2>/dev/null; do
  count=$((count + 1))
  if [ $count -ge $MAX_RETRIES ]; then
    echo "Error: Database at ${DB_HOST}:${DB_PORT} not available after ${MAX_RETRIES} attempts"
    exit 1
  fi
  sleep $RETRY_INTERVAL
done
echo "Database at ${DB_HOST}:${DB_PORT} is ready"

if [ "$ASPNETCORE_ENVIRONMENT" = "Development" ]; then
  echo "Starting in DEVELOPMENT mode with hot reload..."
  exec dotnet watch run --project /app --no-launch-profile -- --api
else
  echo "Starting in PRODUCTION mode..."
  exec dotnet minipdv.dll --api
fi
