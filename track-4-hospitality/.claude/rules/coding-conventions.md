# Coding conventions

## Backend (C#, `backend/Larchmont.Api`)
- Use file-scoped namespaces (`namespace Larchmont.Api.Services;`). `ImplicitUsings` and `Nullable` are on.
- Feature layers, each with an interface and an implementation:
  - Controllers (`Controllers/`): `[ApiController]`, `[Route("api/<resource>")]`. Return `ActionResult<T>`, use `CreatedAtAction` for POST. Call one service and return its DTO. No rules here.
  - Services (`Services/`): `IFooService` / `FooService`. Constructor-inject repositories and `IValidator<T>`. Start with `await _validator.ValidateAndThrowAsync(request)`.
  - Repositories (`Repositories/`): `IFooRepository` / `FooRepository`, wrapping `LarchmontDbContext`.
- Register each new service and repository in `Program.cs` with `AddScoped`. There is no assembly scanning.
- Throw `NotFoundException`, `ConflictException` or `BusinessRuleException` from `Infrastructure/Exceptions.cs`. Don't throw bare `Exception`.
- Mappers (`Mappers/Mappers.cs`): extension methods `ToDto()` on the entity, and `Apply(request)` to copy a request onto an entity. `Apply` trims strings and upper-cases codes.
- DTOs (`Dtos/Dtos.cs`): `record` types.
- Validators (`Validators/Validators.cs`): one `AbstractValidator<TRequest>` per request type.
- EF mapping lives in `Data/LarchmontDbContext.cs`: `ToTable` with snake_case names, `HasMaxLength` on strings, `decimal(10,2)` for money, unique indexes for natural keys.
- Log with `ILogger<T>` (see `ApiExceptionHandler`). The project does not use Serilog.

## Database
- Migrations are numbered `V00N__description.sql` in `backend/db/migrations/`. They are embedded and run by DbUp at startup.
- Every schema change is a new migration. The EF model must match its column types, lengths and indexes.

## Frontend (`frontend/src`)
- TypeScript strict, with `noUnusedLocals` and `noUnusedParameters`.
- HTTP calls go in `api/<area>.ts` and use `getJson` from `api/client.ts`. Use relative paths such as `/api/hotels`. The Vite proxy routes them. `getJson` throws `ApiError`, whose message is the ProblemDetails `detail`.
- Load data with `useAsync` (`hooks/useAsync.ts`). Wrap the result in `StateView` (`components/StateView.tsx`) so loading, error and empty states are all handled.
- One page per screen in `pages/`, routed in `App.tsx`.
- State is local to components or hooks. Don't add Redux.
