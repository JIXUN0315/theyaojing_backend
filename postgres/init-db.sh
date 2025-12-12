#!/bin/bash
set -e

echo "⚙️ Restoring database from backup.dump..."
DB_NAME=${POSTGRES_DB:-mydb}
DB_USER=${POSTGRES_USER:-myuser}

psql -U "$DB_USER" -d "$DB_NAME" -f /docker-entrypoint-initdb.d/schema/init.sql

echo "✅ Restore completed."