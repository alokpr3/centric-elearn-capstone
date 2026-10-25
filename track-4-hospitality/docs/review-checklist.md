# Review checklist

Work through every item for the code you are reviewing. For each one write Pass, Fail or Not applicable, and point to the file and line.

## Correctness
- [ ] The change does what the spec's acceptance criteria say, and every criterion has a test.
- [ ] Business rules live in the service layer, not in controllers or repositories.
- [ ] Invalid input and invalid state changes return a clear 4xx ProblemDetails, never a 500.
- [ ] Date and night counting is right at the edges (check-in day, check-out day, same-day turnover).
- [ ] Tests are reliable and repeatable.

## Data
- [ ] Every schema change is a new migration; no applied migration was edited.
- [ ] The migration and the EF model agree on column types, lengths, nullability and indexes.
- [ ] List endpoints run a fixed number of queries however many rows they return.
- [ ] Two people changing the same record at the same moment cannot both succeed silently.

## Security and privacy
- [ ] No secret, key or password is committed in any file, config or compose file.
- [ ] No response returns fields the caller does not need (secrets, hashes, full personal data).
- [ ] Personal data is shown only to the roles that need it, and nothing is stored that the business has no right to keep.
- [ ] Anything that tracks staff or guests has a lawful purpose and stops when the person is off duty.

## Tests
- [ ] Each test fails if the behaviour it names is broken (try breaking the code to check).
- [ ] No test depends on the current date, time zone or the order tests run in.
- [ ] New code has a test that fails on the old code.

## Delivery
- [ ] `dotnet build`, `dotnet test` and `npm run build` pass from a clean checkout.
- [ ] The CI workflow runs the same steps and its paths and versions match the repo.
- [ ] The README and CLAUDE.md match how the project is really built and run.
- [ ] Loading, empty and error states exist on every new screen.
