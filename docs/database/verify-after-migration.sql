-- Run in Supabase SQL Editor after `dotnet ef database update` to confirm schema.

-- Latest applied EF migration name
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId";

-- Identity + domain tables (public schema)
SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'public'
  AND table_type = 'BASE TABLE'
ORDER BY table_name;
