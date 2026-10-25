# Deliverables - Northgate Freight

Tick each box when the file exists at exactly this path. Paths are relative to this folder.

## Documents
- [ ] `docs/onboarding-brief.md` - architecture plus one request traced end to end
- [ ] `docs/specs/shipment-tracking.md` - spec written from `docs/stakeholder-brief.md`, using `docs/specs/_template.md`
- [ ] `docs/prompt-journal.md` - at least 15 entries (prompt, mode used, what came back, accepted / rejected / rewritten)
- [ ] `docs/mcp-security.md` - the permission design for your MCP server
- [ ] `docs/agent-map.md` - which agent built what
- [ ] `docs/debug-report.md` - NGF-212, including the hypothesis you discarded
- [ ] `docs/refactor-note.md` - the import service refactor
- [ ] `docs/perf-note.md` - Performance and concurrency note (before and after)
- [ ] `docs/review-report.md` - reviewer agent run against `docs/review-checklist.md`
- [ ] `docs/ai-usage-reflection.md` - one page, written last

## Claude Code setup
- [ ] `CLAUDE.md` - corrected and specific to this repo
- [ ] `.claude/rules/*` or a nested `CLAUDE.md`
- [ ] `.claude/skills/` - at least 2 skills
- [ ] `.claude/agents/` - at least 5 agents: backend, frontend, test, reviewer, db-analyst
- [ ] `.claude/settings.json` - with hooks (block write SQL and secrets; build and test after edits; update `CLAUDE.md` after commits)
- [ ] `.mcp.json`

## MCP server
- [ ] `mcp/` - the stdio server
- [ ] `mcp/tests/` - negative tests: write, DDL, multiple statements, a personal-data column, an out-of-scope region and `INTO OUTFILE` are all refused
- [ ] `db-setup/03-create-mcp-user.sql`
- [ ] `db-setup/02-mcp-views.sql`

## Pipeline and run files
- [ ] `.github/workflows/ci.yml` - fixed, plus a job that runs the MCP negative tests
- [ ] `compose.yaml` - fixed
- [ ] `README.md` - updated

## This track: shipment tracking
- [ ] Status machine with at least 8 statuses; delivered and other final states need proof of delivery
- [ ] `POST /api/consignments/{id}/scans`
- [ ] `POST /api/consignments/{id}/status`
- [ ] `GET /api/consignments/{id}/eta` - the single call that answers "when, or what next"
- [ ] Exceptions worklist screen in `frontend/src/pages/`
- [ ] Network dashboard screen (on-time % per depot) in `frontend/src/pages/`
- [ ] MCP business tools: `consignments_in_exception`, `depot_on_time_rate`, `late_by_customer`, `scan_gaps` (plus a guarded `run_readonly_query`)
- [ ] NGF-212 fixed with a regression test that fails on the original code

## Done means
- [ ] All tests pass, including at least 25 named after acceptance criteria (for example `AC_04_CannotSkipTwoStatuses`)
- [ ] The MCP negative tests pass
- [ ] CI is green when run locally with `act` or by running the same steps
- [ ] Every issue you found is fixed or explained in your docs
- [ ] Every item above is ticked
