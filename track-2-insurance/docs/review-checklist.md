# Review checklist

Use this for every change before it ships.

## Correctness
- [ ] Does the change do what the spec and acceptance criteria say, including the error cases?
- [ ] Are invalid inputs and invalid state changes rejected with a clear 4xx ProblemDetails response?
- [ ] Are money amounts handled as decimals, never floats?
- [ ] Are dates and times handled in one consistent way, and are the tests reliable and repeatable?

## Data and concurrency
- [ ] If two people change the same record at the same moment, what happens? Is there a guard?
- [ ] Do list endpoints run a fixed number of queries regardless of page size?
- [ ] Do the SQL migrations match the EF Core model (types, lengths, nullability, indexes)? Is every schema change a new migration, never an edit to an old one?

## Security and privacy
- [ ] Are there any secrets, keys or passwords in the code, config files or compose files?
- [ ] Does any endpoint return more than the caller needs (password hashes, full personal data)?
- [ ] Is personal data shown only to the roles that need it? Is anything stored that policy does not allow (for example full card numbers)?
- [ ] Is every SQL statement parameterised?

## Tests
- [ ] Does every new rule have a test named after its acceptance criterion?
- [ ] Does each test actually assert the behaviour in its name?
- [ ] Is every test deterministic (no dependence on today's date, time zone or run order)?

## Delivery
- [ ] Does the build pass locally exactly as the CI pipeline runs it?
- [ ] Are the README, CLAUDE.md and any docs still true after this change?
