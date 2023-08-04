CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

DO
$do$
BEGIN
   IF NOT EXISTS (
      SELECT FROM pg_catalog.pg_database
      WHERE datname = 'rankings') THEN

      CREATE DATABASE rankings;
   END IF;
END
$do$;
