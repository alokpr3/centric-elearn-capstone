# Larchmont Hotels - agent briefing

Internal reservations system for six Larchmont hotels. Background is in README.md.

## Stack
- API: ASP.NET Core 8 Web API, controllers with `[ApiController]` and attribute routes under `api/`. Swashbuckle for Swagger.
- Data: EF Core 8 with Pomelo.EntityFrameworkCore.MySql, MySQL 8.4.
- Schema: DbUp runs `backend/db/migrations/*.sql` (embedded resources) at startup. It is skipped when the environment is `Testing`.
- Validation: FluentValidation 11, run from services with `ValidateAndThrowAsync`.
- Frontend: Vite 6 + React 19 + TypeScript (strict) in `frontend/`, react-router-dom 7. No Redux, no test runner.
- Tests: xUnit with Moq, Microsoft.AspNetCore.Mvc.Testing and the EF Core InMemory provider.

## Commands (run from this folder)
- Create MySQL users (once): `mysql -h 127.0.0.1 -P 3306 -u root -p < db-setup/01-create-users.sql`
- Start MySQL 8.4 with Docker: `docker compose up -d mysql`
- Run API: `dotnet run --project backend/Larchmont.Api` (Swagger at http://localhost:5080/swagger)
- Run frontend: `cd frontend && npm install && npm run dev` (http://localhost:5173, `/api` proxied to 5080)
- Test: `dotnet test backend`
- Build frontend: `cd frontend && npm run build`

## Layout
- `backend/Larchmont.Api/` - Controllers, Services, Repositories, Domain (entities), Data (DbContext), Dtos, Mappers, Validators, Infrastructure (exceptions)
- `backend/Larchmont.Tests/` - `UnitTests.cs` (services with mocks), `ApiTests.cs` (HTTP tests on an in-memory host)
- `backend/db/migrations/` - numbered SQL migrations, `V001` to `V004`
- `db-setup/` - one-off MySQL user scripts
- `frontend/src/` - `api/` fetch wrappers, `pages/`, `components/`, `hooks/`

## Conventions
- Services throw typed exceptions and `ApiExceptionHandler` maps them: `NotFoundException` 404, `ConflictException` 409, `BusinessRuleException` 422, FluentValidation errors 400. Services do not return error objects.
- Only repositories touch `LarchmontDbContext`. Business rules live in services, not controllers.
- Stay dates are `DateOnly`. Audit timestamps are UTC (`DateTime.UtcNow`). `BookingService.CancelAsync` is the exception: it uses `DateTime.Now`.
- Never edit a migration that has been applied. Add the next `V00N__name.sql`.
- Connection string: `ConnectionStrings:Larchmont`, overridden by the env var `ConnectionStrings__Larchmont`. Never commit passwords or API keys.
- Coding and test patterns: `.claude/rules/`.

## Known issues
- `.github/workflows/ci.yml` is stale: it uses .NET 6 and a `web/` folder. The API is .NET 8 and the frontend is `frontend/`.
- `backend/Larchmont.Api/appsettings.Development.json` and `compose.yaml` commit plaintext credentials. Rotate them and move them to env vars.
- No entity has a concurrency token, so two concurrent edits can overwrite each other.
- The project targets .NET 8. If `dotnet` resolves to .NET 10 only, `dotnet test` fails with runtime errors. Install the .NET 8 SDK.
