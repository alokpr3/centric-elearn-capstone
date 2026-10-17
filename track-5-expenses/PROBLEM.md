# Track: Team Expense Tracker

## The problem

A team goes on trips and lunches together. One person pays and everyone else says they will pay back later. Nobody remembers who owes what. Small amounts are lost and friends feel awkward.

## Who uses it

1. **Team member** - adds an expense and sees what they owe.
2. **Trip organiser** - sees all expenses for the trip.
3. **Accountant** - checks that the numbers add up.

## Step 1: Fix the starter code

The starter code has 5 problems. It does not build, some results are wrong, and one test fails. Use Claude Code to find and fix them. Write each problem you found in FIXES.md in your track folder.

## Must have

1. Show the total owed by each person across all expenses.
2. Delete a wrong expense.
3. Group expenses into trips or events.
4. Show who should pay whom (a short list of payments to settle up).
5. Check the input: no negative amounts, no empty names.

## Nice to have

1. Unequal split (for example 60% and 40%).
2. Export the summary as a CSV file.
3. Show expenses by date range.

## Done means

- [ ] `dotnet build` works with no errors
- [ ] All tests pass (`dotnet test`)
- [ ] All 5 must-haves work in the browser
- [ ] FIXES.md is written (one line per problem: what was wrong, how you fixed it)
- [ ] The .claude folder is committed

## Claude Code and demo

Use the [Claude Code checklist](../README.md#claude-code-you-must-use) and the [Day 8 demo steps](../README.md#day-8-demo-5-minutes).
For your demo, step 1 is "who owes what, nobody remembers". Step 3 is "add an expense of 100 shared by 3 people and show the split".
