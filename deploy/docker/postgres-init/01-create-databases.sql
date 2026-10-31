-- Postgres init for recovery-backoffice
-- This file runs only on first boot (when the data directory under /var/lib/postgresql is empty).
-- The `recovery` database itself is created by POSTGRES_DB= in the compose env;
-- we just add the extensions the framework relies on.

\connect recovery

CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- pg_trgm is used by the full-text search GIN indexes (chat search).
CREATE EXTENSION IF NOT EXISTS pg_trgm;
