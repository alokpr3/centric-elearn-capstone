# Track: Office Canteen Food Ordering

## The problem

At lunch time 80 people queue at the office canteen. Orders are shouted across the counter and often get lost. The cook does not know how much of each dish to make. Employees do not know the price until they pay.

## Who uses it

1. **Employee** - picks food from the menu and places an order.
2. **Canteen staff** - sees new orders and prepares them.
3. **Manager** - sees how much was sold today.

## Step 1: Fix the starter code

The starter code has 5 problems. It does not build, some results are wrong, and one test fails. Use Claude Code to find and fix them. Write each problem you found in FIXES.md in your track folder.

## Must have

1. Show the order total in the browser before the order is placed.
2. Mark an order as Preparing, then Ready, then Collected.
3. A kitchen page that lists open orders, oldest first.
4. Add and remove menu items, and mark an item as sold out.
5. A daily report: number of orders and total money.

## Nice to have

1. Order history for one employee.
2. A cut-off time: no orders after 2 PM.
3. Show the most popular dish today.

## Done means

- [ ] `dotnet build` works with no errors
- [ ] All tests pass (`dotnet test`)
- [ ] All 5 must-haves work in the browser
- [ ] FIXES.md is written (one line per problem: what was wrong, how you fixed it)
- [ ] The .claude folder is committed

## Claude Code and demo

Use the [Claude Code checklist](../README.md#claude-code-you-must-use) and the [Day 8 demo steps](../README.md#day-8-demo-5-minutes).
For your demo, step 1 is "the queue and the lost orders". Step 3 is "place an order with 3 of one dish and check the total".
