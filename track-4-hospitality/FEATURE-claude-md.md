# Feature: correct CLAUDE.md and add rules files

## Why
The current CLAUDE.md describes a different stack: ASP.NET 6 minimal APIs, MediatR, Dapper, SQL Server LocalDB, NUnit, and a Create React App client in `client/`. None of that is in the repo. Agents working from it will write the wrong code.

## What the repo actually uses (checked against the files)
| Area | Real |
|---|---|
| API | .NET 8 (`net8.0`), ASP.NET Core controllers with `[ApiController]` and attribute routes under `api/` |
| Data | EF Core 8.0.13 + Pomelo.EntityFrameworkCore.MySql 8.0.3, MySQL 8.4 (`MySqlServerVersion 8.4.0`) |
| Schema | DbUp 6.1.0 (`dbup-mysql`) runs `backend/db/migrations/*.sql` (embedded resources) at startup, skipped when env is `Testing` |
| Validation | FluentValidation 11, called from services with `ValidateAndThrowAsync` |
| Errors | Services throw `NotFoundException` (404), `ConflictException` (409), `BusinessRuleException` (422); `ApiExceptionHandler` maps them. No `Result<T>`. |
| Frontend | Vite 6, React 19, TypeScript 5.6 strict, react-router-dom 7, in `frontend/`. No Redux, no test runner. Dev proxy `/api` to `localhost:5080`. |
| Tests | xUnit 2.9.2 + Moq + Microsoft.AspNetCore.Mvc.Testing + EF Core InMemory. Not NUnit or MSTest. |
| Test names | `Method_Scenario_Expected` (e.g. `Quote_MultipliesNightlyRateByNights`), not `Given_When_Then` |
| Commands | README: `dotnet run --project`, `npm install`, `npm run dev`, `dotnet test`. Compose starts MySQL 8.4 only. |

## Problems found while reading (not fixed here)
- `backend/Larchmont.Api/appsettings.Development.json` is committed with a DB password, a channel-manager API key and an SMTP password. These are already on `origin`; they need rotating and moving to env vars or user secrets.
- `compose.yaml` commits `MYSQL_ROOT_PASSWORD`.
- `.github/workflows/ci.yml` uses .NET `6.0.x` and `working-directory: web`. Neither matches the repo.
- No entity has a concurrency token (`RowVersion` or similar), so the review checklist's concurrency item fails.
- Date handling is mixed: `BookingService.CancelAsync` uses `DateTime.Now`, the rest uses `DateTime.UtcNow`. Two `UnitTests.cs` cancel tests depend on the current date.

## Steps
1. Write this plan (done).
2. Rewrite `CLAUDE.md` under 60 lines: stack, commands from the real README and compose file, layout, the conventions the code really follows, and known issues. No secrets copied from the config files.
3. Create `.claude/rules/coding-conventions.md`: backend and frontend patterns taken from the existing files (controller, service, repository, mapper, validator, `useAsync`, `getJson`, `StateView`).
4. Create `.claude/rules/testing-conventions.md`: xUnit naming, unit vs API tests, the InMemory limits, and the rule that performance or concurrency claims need a MySQL test.
5. Verify: line count of CLAUDE.md, every path mentioned exists, and run `dotnet test backend` and `npm run build` in `frontend/` to check the commands.

## Decisions
- Commands use the paths you gave (`backend/Larchmont.Api`, `frontend`), which run from the track folder. The README uses `cd backend` first; both work.
- Named the test framework as xUnit, since that's what `Larchmont.Tests.csproj` references.
- Rules files have no `paths:` filter, so they load in every session.
- Nothing is committed or pushed. The change stays in the working tree for you to review.
