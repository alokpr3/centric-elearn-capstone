# Track: Clinic Appointment Booking

## The problem

A small clinic takes bookings by phone. Two patients are often booked with the same doctor at the same time. Patients wait in a crowd while the doctor is busy. The receptionist keeps a notebook and loses pages.

## Who uses it

1. **Patient** - picks a doctor and a time slot.
2. **Receptionist** - sees all bookings for the day.
3. **Doctor** - sees only their own schedule.

## Step 1: Fix the starter code

The starter code has 5 problems. It does not build, some results are wrong, and one test fails. Use Claude Code to find and fix them. Write each problem you found in FIXES.md in your track folder.

## Must have

1. Show free and booked slots for a doctor on a chosen day.
2. Cancel an appointment (the slot becomes free again).
3. A schedule page: all appointments for one doctor, sorted by time.
4. Do not allow a booking in the past.
5. Show a clear error in the browser when a slot is taken.

## Nice to have

1. Search doctors by speciality.
2. Reschedule an appointment to a new time.
3. Show a daily summary (how many visits per doctor).

## Done means

- [ ] `dotnet build` works with no errors
- [ ] All tests pass (`dotnet test`)
- [ ] All 5 must-haves work in the browser
- [ ] FIXES.md is written (one line per problem: what was wrong, how you fixed it)
- [ ] The .claude folder is committed

## Claude Code and demo

Use the [Claude Code checklist](../README.md#claude-code-you-must-use) and the [Day 8 demo steps](../README.md#day-8-demo-5-minutes).
For your demo, step 1 is "the notebook and the double bookings". Step 3 is "book a slot, then try to book it again and show the error".
