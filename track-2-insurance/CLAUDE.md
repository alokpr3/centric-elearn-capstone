# Harbourline Claims Platform

Internal claims system for Harbourline Insurance (home and motor).

## Stack
- API: ASP.NET Core 8 with CQRS using MediatR. Every feature is a command or query handler in `src/Harbourline.Application`.
- Data access: Dapper with stored procedures. Never use EF Core in this repo.
- Database: MongoDB (collections: `policies`, `claims`, `handlers`).
- Auth: JWT bearer tokens. Every controller has `[Authorize]`; get the handler id from the `sub` claim.
- UI: Next.js 14 with Redux Toolkit. Pages live in `frontend/pages`.

## Commands
- Run API: `dotnet run --project src/Harbourline.Web`
- Run tests: `dotnet test tests/Harbourline.UnitTests` (integration tests are in `tests/Harbourline.IntegrationTests` and need Docker)
- Run UI: `cd frontend && yarn dev` (port 3000)
- Migrations: `dotnet ef database update`
- Lint: `npm run lint:fix` in the repo root

## Conventions
- Business rules belong in the controllers, keep services thin.
- Money is stored as a `double`.
- All dates are stored in local time (Europe/London).
- Do not add new NuGet packages without asking Priya.
- Reports are generated nightly by the `ReportJob` worker and cached in Redis; the API only reads the cache.
