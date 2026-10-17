# Track: Library Book Borrowing

## The problem

The college library still writes loans in a paper book. Staff cannot tell which books are out or who is late. Students argue about due dates. The librarian spends every Friday adding up fines by hand.

## Who uses it

1. **Student** - looks for a book and borrows it.
2. **Librarian** - sees who has which book and who is late.
3. **Admin** - adds books and sets the fine amount.

## Step 1: Fix the starter code

The starter code has 5 problems. It does not build, some results are wrong, and one test fails. Use Claude Code to find and fix them. Write each problem you found in FIXES.md in your track folder.

## Must have

1. Return a book (a return date is saved and the copy is available again).
2. Show a fine when a book comes back late. The fine is per late day.
3. A page that lists all loans: who, which book, due date, late or not.
4. Add a new book (title, author, copies).
5. Search the book list by title or author.

## Nice to have

1. Show overdue loans in red.
2. Let a student renew a loan once (14 more days).
3. A page with each student's loan history.

## Done means

- [ ] `dotnet build` works with no errors
- [ ] All tests pass (`dotnet test`)
- [ ] All 5 must-haves work in the browser
- [ ] FIXES.md is written (one line per problem: what was wrong, how you fixed it)
- [ ] The .claude folder is committed

## Claude Code and demo

Use the [Claude Code checklist](../README.md#claude-code-you-must-use) and the [Day 8 demo steps](../README.md#day-8-demo-5-minutes).
For your demo, step 1 is "the paper book and the pain". Step 3 is "list books, borrow, return late, show the fine".
