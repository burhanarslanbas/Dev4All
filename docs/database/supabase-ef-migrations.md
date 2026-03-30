# Supabase + EF Core migrations (Dev & Prod)

This project uses **Entity Framework Core** with **Npgsql** against Supabase-hosted PostgreSQL. Migrations live in source control under `backend/src/Infrastructure/Dev4All.Persistence/Migrations/`.

## Connection strings

Read connection strings from **Supabase Dashboard** → **Project Settings** → **Database**.

| Mode | Typical port | When to use |
|------|----------------|-------------|
| **Transaction pooler** (PgBouncer) | `6543` | App runtime, many short connections |
| **Direct** | `5432` | Long sessions, migrations (often simpler) |

Npgsql connection string parameters (examples):

- `Host=<project-ref>.pooler.supabase.com` or `Host=db.<project-ref>.supabase.co`
- `Port=6543` (pooler) or `5432` (direct)
- `Database=postgres`
- `Username=postgres` (or pooler user if using Supabase pooler user)
- `Password=<secret>`
- `SSL Mode=Require` (recommended for Supabase)

**Never commit** real passwords. Use **User Secrets** locally and **CI / hosting secrets** in pipelines.

### Local User Secrets (WebAPI)

The WebAPI project has a `UserSecretsId`. From the repo root:

```bash
cd backend/src/Presentation/Dev4All.WebAPI
dotnet user-secrets set "Database:ConnectionString" "<your-supabase-connection-string>"
```

Environment variable equivalent (double underscore for nested keys):

```bash
set DATABASE__CONNECTIONSTRING=<your-supabase-connection-string>
```

## EF Core CLI (pinned version)

Restore the local tool manifest once (repo root):

```bash
dotnet tool restore
```

`dotnet-ef` is pinned in [`.config/dotnet-tools.json`](../../.config/dotnet-tools.json) to match `Microsoft.EntityFrameworkCore.*` **10.0.x**.

## Design-time DbContext

[`Dev4AllDbContextFactory`](../../backend/src/Infrastructure/Dev4All.Persistence/Context/Dev4AllDbContextFactory.cs) implements `IDesignTimeDbContextFactory<Dev4AllDbContext>` so:

- `dotnet ef migrations add` works **without** bootstrapping the full API when only the model changed.
- For `database update`, set `DATABASE__CONNECTIONSTRING` (or User Secrets) so the tool targets the correct database.

## Create a new migration

From **repository root** (where `.config/dotnet-tools.json` exists):

```bash
dotnet tool restore
dotnet ef migrations add <MigrationName> ^
  --project backend/src/Infrastructure/Dev4All.Persistence/Dev4All.Persistence.csproj ^
  --startup-project backend/src/Presentation/Dev4All.WebAPI/Dev4All.WebAPI.csproj ^
  --context Dev4AllDbContext
```

On Linux/macOS, replace `^` with `\`.

## Apply migrations (Development)

1. Set the **development** Supabase connection string (User Secrets or env).
2. Run:

```bash
dotnet ef database update ^
  --project backend/src/Infrastructure/Dev4All.Persistence/Dev4All.Persistence.csproj ^
  --startup-project backend/src/Presentation/Dev4All.WebAPI/Dev4All.WebAPI.csproj ^
  --context Dev4AllDbContext
```

Alternatively, pass the connection string without storing it in configuration:

```bash
dotnet ef database update ^
  --project backend/src/Infrastructure/Dev4All.Persistence/Dev4All.Persistence.csproj ^
  --startup-project backend/src/Presentation/Dev4All.WebAPI/Dev4All.WebAPI.csproj ^
  --context Dev4AllDbContext ^
  --connection "<your-supabase-connection-string>"
```

3. Verify in Supabase **SQL Editor** using [verify-after-migration.sql](./verify-after-migration.sql) (checks `__EFMigrationsHistory` and table list).

4. Smoke-test the API: `GET /health`, then auth/register flows if Identity tables are new.

### WebAPI tooling package

`Dev4All.WebAPI` references `Microsoft.EntityFrameworkCore.Design` (private assets) so `dotnet ef` can use it as the **startup project** alongside the Persistence project that contains migrations.

## Production runbook

### Before deploy

1. **Code review**: migration + domain changes in the same PR; flag destructive changes (drop/rename).
2. **Backup**: take a Supabase backup or snapshot per your org policy before risky changes.
3. **Order of deploy**: prefer **apply database migration first**, then deploy the API version that expects the new schema (or ship backward-compatible API + migration together).

### Apply migration to production

1. Set **production** connection string only in the secure channel (never in git):
   - CI secret `DATABASE__CONNECTIONSTRING`, or
   - Manual run from an operator machine with env set.
2. Run the same `dotnet ef database update` command as above against prod (or use the optional manual GitHub Actions workflow).
3. Confirm `__EFMigrationsHistory` and application health (logs, error rate, critical endpoints).

### Rollback expectations

- `dotnet ef database update <PreviousMigration>` can **drop data** or fail on live systems.
- Prefer **forward-fix** migrations and expand/contract schema patterns (nullable column → backfill → enforce).

## Optional: CI manual migration job

See [`.github/workflows/ef-database-update.yml`](../../.github/workflows/ef-database-update.yml) — **workflow_dispatch** only. Configure a GitHub Environment with required reviewers and store `DATABASE__CONNECTIONSTRING` as a secret.
