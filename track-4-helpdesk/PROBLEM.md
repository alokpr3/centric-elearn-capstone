# Track: IT Helpdesk Tickets

## The problem

Staff send IT problems by email, chat and phone. Nothing is in one place. A broken email server waits in line behind a request for a new mouse. The IT team cannot say how many problems are open.

## Who uses it

1. **Employee** - raises a ticket and checks its status.
2. **IT agent** - picks up the most urgent tickets and closes them.
3. **IT manager** - sees how many tickets are open and how fast they are solved.

## Step 1: Fix the starter code

The starter code has 5 problems. It does not build, some results are wrong, and one test fails. Use Claude Code to find and fix them. Write each problem you found in FIXES.md in your track folder.

## Must have

1. Change a ticket status (Open, In Progress, Closed).
2. Filter the list by status and by priority.
3. A ticket details page with the full description.
4. Assign a ticket to an IT agent.
5. A summary page: open tickets per priority.

## Nice to have

1. Add comments on a ticket.
2. Show a warning when an Urgent ticket is older than 1 hour.
3. Search tickets by title.

## Done means

- [ ] `dotnet build` works with no errors
- [ ] All tests pass (`dotnet test`)
- [ ] All 5 must-haves work in the browser
- [ ] FIXES.md is written (one line per problem: what was wrong, how you fixed it)
- [ ] The .claude folder is committed

## Claude Code and demo

Use the [Claude Code checklist](../README.md#claude-code-you-must-use) and the [Day 8 demo steps](../README.md#day-8-demo-5-minutes).
For your demo, step 1 is "email, chat, phone and no order". Step 3 is "raise a ticket and check it shows up in the list at once".
