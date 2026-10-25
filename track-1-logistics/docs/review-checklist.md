# Review checklist

Use this for every review of a change in this repository.

## Correctness
- [ ] The change does what the spec says; each acceptance criterion has a test.
- [ ] Invalid input returns a clear 4xx ProblemDetails response, never a 500.
- [ ] Rules are enforced in the service layer, not only in the UI.
- [ ] Dates and times are handled in one consistent way, and the tests are reliable and repeatable.
- [ ] Money, weights and percentages are rounded in one place.

## Data and queries
- [ ] No query runs inside a loop; list endpoints use a bounded number of queries.
- [ ] Reads that feed reports exclude records that should not count.
- [ ] Concurrent updates to the same record cannot both succeed silently.
- [ ] Every SQL migration is new; no existing migration was edited. The model and the schema agree on lengths, nullability and indexes.

## Security and privacy
- [ ] No secret, key, token or password is committed, including in config and compose files.
- [ ] Endpoints return only the fields the caller needs; no password hashes, no unnecessary personal data.
- [ ] Personal data (names, phone numbers, addresses, locations) is collected and shown only where there is a lawful reason.
- [ ] Anything that touches the database for an assistant or tool is read-only and scoped.

## Tests
- [ ] New tests fail when the behaviour they cover is broken (check the assertion, not just the name).
- [ ] No test depends on the current date or time.
- [ ] Tests run on a clean checkout with the documented commands.

## Delivery
- [ ] CI is green with the same steps run locally.
- [ ] README and CLAUDE.md match what the code actually does.
- [ ] Changes are small enough to review in one sitting.
