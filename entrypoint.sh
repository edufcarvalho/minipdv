#!/bin/sh
DB_HOST="${DB_SERVER%%,*}"
DB_PORT="${DB_SERVER##*,}"
[ "$DB_PORT" = "$DB_SERVER" ] && DB_PORT=1433
DB_HOST="${DB_HOST:-localhost}"

echo "Waiting for database at ${DB_HOST}:${DB_PORT}..."
until bash -c "exec 3<>/dev/tcp/${DB_HOST}/${DB_PORT}" 2>/dev/null; do
  sleep 1
done
echo "Database port is ready"

exec dotnet minipdv.dll --api
