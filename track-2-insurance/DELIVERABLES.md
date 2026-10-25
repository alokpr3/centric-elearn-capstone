# Deliverables - Harbourline claims workflow

Tick every box before you submit. Paths are relative to this track folder. Anything not listed earns no marks.

## Documents
- [ ] `docs/onboarding-brief.md` - architecture plus one request traced end to end
- [ ] `docs/specs/claims-workflow.md` - spec written from `docs/stakeholder-brief.md`, before any code
- [ ] `docs/prompt-journal.md` - at least 15 entries (prompt, mode, what came back, accepted / rejected / rewritten)
- [ ] `docs/mcp-security.md` - the permission design for the MCP server
- [ ] `docs/agent-map.md` - which agent built what
- [ ] `docs/debug-report.md` - the HLI-87 investigation, including the hypothesis you discarded
- [ ] `docs/refactor-note.md` - the importer refactor
- [ ] `docs/perf-note.md` - Performance and concurrency note (before and after). Proof of performance, schema and concurrency fixes needs a test against MySQL or the EF Core SQL log (the existing tests use an in-memory database).
- [ ] `docs/review-report.md` - reviewer agent run against `docs/review-checklist.md`
- [ ] `docs/ai-usage-reflection.md` - one page, written last

## Claude Code setup
- [ ] `CLAUDE.md` - corrected, specific to this repo
- [ ] `.claude/rules/*` or a nested `CLAUDE.md`
- [ ] `.claude/skills/` - at least 2
- [ ] `.claude/agents/` - at least 5: backend, frontend, test, reviewer, db-analyst
- [ ] `.claude/settings.json` - with hooks (block write SQL and secrets; build and test after edits; update `CLAUDE.md` after a commit)
- [ ] `.mcp.json`

## MCP server
- [ ] `mcp/` - the stdio server
- [ ] `mcp/tests/` - negative tests: write, DDL, multiple statements, a PII column, an out-of-scope region and `INTO OUTFILE` are all refused
- [ ] `db-setup/03-create-mcp-user.sql`
- [ ] `db-setup/02-mcp-views.sql`

## Pipeline and run files
- [ ] `.github/workflows/ci.yml` - fixed, plus a job that runs the MCP negative tests
- [ ] `compose.yaml` - complete
- [ ] `README.md` - updated

## Track 2 specific: claims workflow
- [ ] Workflow: triage, then assess, then approve or decline, then settle, then close. Reopening needs a reason.
- [ ] Approval limits per handler grade, and the person who assessed a claim cannot approve it
- [ ] Final states that need a settlement amount are enforced
- [ ] Invalid moves return a clear 4xx ProblemDetails response
- [ ] Single call: `GET /api/claims/{id}/next-actions`
- [ ] Screen: handler queue
- [ ] Screen: limits breach report
- [ ] Ticket `docs/bug-HLI-87.md` resolved, with a regression test that fails on the original code
- [ ] MCP business tools: `claims_over_authority`, `ageing_by_stage`, `loss_ratio_by_product`, `reopened_claims`, plus a guarded `run_readonly_query`
- [ ] Tests: all pass, at least 25 named after acceptance criteria (for example `AC_04_AssessorCannotApprove`)
- [ ] Every finding in your review is fixed or explained in `docs/review-report.md`
