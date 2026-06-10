-- One database per service so FluentMigrator VersionInfo does not collide
CREATE DATABASE dotnetcv_users;
CREATE DATABASE dotnetcv_orders;
-- dotnetcv (main) already exists via POSTGRES_DB; used by products-service
