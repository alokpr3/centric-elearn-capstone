# LH-56: Occupancy report overstates occupancy

Reporter: Sofia Lindqvist, Director of Rooms
Priority: High (revenue manager is setting rates from this report)
Component: Reports

## Symptom

The occupancy report shows far higher occupancy than the hotels and the front-desk night audits report.

For Larchmont Edinburgh, week of 1 to 7 October 2026 (seed data reference week):

| | Room nights sold | Occupancy |
|---|---|---|
| Report says | 69 | 49.3% |
| Night-audit count says | 45 | 32.1% |

Edinburgh has 20 rooms, so 7 nights is 140 available room nights.

For the whole group, same week (120 rooms, 840 room nights):

| | Room nights sold | Occupancy |
|---|---|---|
| Report says | 334 | 39.8% |
| Night-audit count says | 226 | 26.9% |

One day, Edinburgh, Friday 2 October 2026:

| | Rooms sold | Occupancy |
|---|---|---|
| Report says | 9 | 45% |
| Night audit says | 5 | 25% |

Four guests checked out of Edinburgh on the morning of 2 October. Their rooms were empty that night and were not sold to anyone.

## Steps to reproduce

1. Start the API against the seeded database.
2. `GET /api/reports/occupancy?hotelId=1&from=2026-10-02&to=2026-10-02`
3. Read `roomNightsSold` and `occupancyPct`.

## Expected

`roomNightsSold` = 5, `occupancyPct` = 25 for that request. For `hotelId=1&from=2026-10-01&to=2026-10-07`: 45 and 32.1.

## Actual

`roomNightsSold` = 9, `occupancyPct` = 45. For the week: 69 and 49.3.
