# Larchmont Hotels - agent briefing

## Stack
- API: ASP.NET Core 6 minimal APIs with MediatR (CQRS: every endpoint is a Command or Query handler).
- Data access: Dapper over stored procedures in SQL Server. Do not use EF Core.
- Mapping: AutoMapper profiles live in `src/Larchmont.Web/Mapping`.
- Logging: Serilog, writes to `logs/larchmont-.txt`.
- Tests: NUnit with a real SQL Server LocalDB instance. Name tests `Given_When_Then`.
- Client: Create React App (JavaScript) in `client/`, state in Redux Toolkit.

## Commands
- Run API: `dotnet run --project src/Larchmont.Web`
- Run client: `cd client && npm start`
- Test: `dotnet test Larchmont.sln --filter Category=Unit`
- Apply schema: `sqlcmd -S localhost -i db/schema.sql`

## Layout
- `src/Larchmont.Web` - API, handlers, validators
- `src/Larchmont.Data` - stored procedure wrappers
- `client/` - React app
- `db/` - schema and stored procedures

## Conventions
- Handlers return `Result<T>`; never throw from a handler.
- All dates are stored as local hotel time, no timezone conversion anywhere.
- Rooms are called "units" in code and the database (`units` table).
- Bookings are called "reservations" (`reservations` table).
- Never edit SQL files that have already been deployed; add a new numbered file.
- Config lives in `appsettings.Production.json`; copy it to run locally.
