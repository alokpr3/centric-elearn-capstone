# Capstone: five inherited codebases

This capstone puts you in the seat of an engineer who inherits a working company application that nobody owns. You read it, write a spec for a feature a stakeholder has asked for, build it with Claude Code, and ship it.
Along the way you build an MCP server that reads the database safely, fix a wrong report, refactor an importer, and review your own work.
You have 2 days. All five tracks use the same stack (ASP.NET Core 8, EF Core, MySQL, React 19 + TypeScript) and the same tasks.

| Track | Company | What you build | Folder |
|---|---|---|---|
| 1 | Northgate Freight (logistics) | Shipment tracking: status rules, exceptions, arrival time in one call | [track-1-logistics](track-1-logistics/) |
| 2 | Harbourline Insurance | Claims workflow: triage, approval limits per handler, settlement | [track-2-insurance](track-2-insurance/) |
| 3 | Kestrel Outfitters (retail) | Returns and refunds: return window, grading, partial refunds, restock | [track-3-retail](track-3-retail/) |
| 4 | Larchmont Hotels | Housekeeping and room-status board with a turnaround target | [track-4-hospitality](track-4-hospitality/) |
| 5 | Corrin Utility Services (field service) | Work orders and engineer scheduling | [track-5-fieldservice](track-5-fieldservice/) |

## Where to start

Start with [CAPSTONE.md](CAPSTONE.md). Then open your track's `README.md`, `docs/stakeholder-brief.md` and `DELIVERABLES.md`, in that order.

## Scoring (100 points)

| Task | Points |
|---|---|
| 1. Onboard and set up Claude Code | 10 |
| 2. MCP server to MySQL | 20 |
| 3. Brief to spec | 15 |
| 4. Build the feature backend | 15 |
| 5. Fix the ticket | 8 |
| 6. Refactor the importer | 6 |
| 7. Feature UI | 6 |
| 8. Tests | 8 |
| 9. Review, harden and ship | 9 |
| Prompt journal and AI usage reflection | 3 |

There are no marks for features beyond the brief.
