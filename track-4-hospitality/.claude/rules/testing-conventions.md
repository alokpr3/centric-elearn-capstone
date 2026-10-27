# Testing conventions

## Framework
- xUnit 2.9 with `[Fact]` and `[Theory]`, `Assert.*` for checks, Moq for mocks. Not NUnit or MSTest.
- Run with `dotnet test backend`. No MySQL is needed for the existing tests.

## Naming
- Name tests `Method_Scenario_Expected`, for example `Quote_CheckOutNotAfterCheckIn_IsRejected`.
- Existing tests from older versions may not match, so follow this pattern for new tests.

## Unit tests (`backend/Larchmont.Tests/UnitTests.cs`)
- Build the service under test by hand: mock each repository interface with Moq, and pass real validators.
- Set up only what the test needs. Use `Verify` to check side effects, such as a booking being added.

## API tests (`backend/Larchmont.Tests/ApiTests.cs`)
- Use `IClassFixture<ApiFactory>`. `ApiFactory` runs the real app in the `Testing` environment, so DbUp is skipped, and swaps in an InMemory database with a unique name per factory.
- Call `factory.SeedBasics()` for reference data, or `Seed(...)` for your own data.

## What InMemory cannot prove
- It does not enforce unique indexes, check constraints or foreign keys. It has no transactions and no concurrency conflicts. It does not use MySQL decimal or date rules.
- So a test that claims performance, query count, concurrency safety or a migration works must run against a real MySQL 8.4 instance, or assert on the EF Core SQL log. No such test exists yet.

## Rules for every test
- Each test must fail when the behaviour it names is broken. Break the code to check.
- Don't use the current date or time. Pass fixed dates such as `new DateOnly(2026, 10, 5)`. Existing tests in `UnitTests.cs` that use `DateTime.UtcNow` or `DateTime.Now` should not be copied.
- Tests must not depend on each other or on run order.
- Every new behaviour needs a test that would fail on the old code.
