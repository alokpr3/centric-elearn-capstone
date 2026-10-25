# HLI-87: Loss ratio report shows different numbers from Finance

**Reported by:** Priya Raman (Head of Claims), on behalf of Raj Mehta, Finance
**Priority:** High - the report goes to the board pack on the 5th
**Component:** Reports / Loss ratio

## Summary
The loss ratio report overstates the loss ratio for several products. Finance's own ledger gives a lower amount paid for the same period, and the two do not reconcile.

## Where I see it
Reports > Loss ratio, period 2025-10-01 to 2026-09-30 (the default). Same numbers from the API:

`GET /api/reports/loss-ratio?from=2025-10-01&to=2026-09-30`

## What the report shows

| Product | Written premium | Paid claims | Loss ratio | Settlements |
|---|---:|---:|---:|---:|
| Motor Third Party Fire & Theft | 14,107.04 | 10,706.17 | 75.9% | 11 |
| Home Basic | 12,650.40 | 11,554.79 | 91.3% | 14 |
| Classic Car | 5,118.25 | 3,790.03 | 74.0% | 6 |
| Motor Comprehensive | 79,165.23 | 36,516.48 | 46.1% | 31 |

Whole book: paid 84,426.57 on written premium 180,339.57, a loss ratio of 46.8%.

## What Finance has
Finance count the claims they have actually paid in the same period, and each claim is paid once:

| Product | Paid claims | Loss ratio | Claims paid |
|---|---:|---:|---:|
| Motor Third Party Fire & Theft | 4,770.20 | 33.8% | 8 |
| Home Basic | 9,020.96 | 71.3% | 13 |
| Classic Car | 2,008.69 | 39.2% | 4 |
| Motor Comprehensive | 31,774.98 | 40.1% | 28 |

Whole book: paid 62,946.35 across 84 claims, a loss ratio of 34.9%.

The premium column matches between us. Only the paid amount differs, and the difference is not spread evenly: some products are barely affected (Motor Van, Landlord Home match exactly), others are out by a lot.

## Steps to reproduce
1. Start the API on the seed data.
2. Call `GET /api/reports/loss-ratio` with the default period.
3. Compare the Paid claims and Settlements columns with the number of distinct paid claims for the product.

## Expected
Paid claims is the sum of what was paid on each claim in the period. The Settlements column should agree with the number of claims paid. For Motor Third Party Fire & Theft that is 8 claims and 4,770.20, a loss ratio of 33.8%.

## Actual
The report shows 11 settlements and 10,706.17 for the same product, a loss ratio of 75.9%.

## Notes
- The numbers have been wrong since the report was introduced; nobody compared it to Finance before.
- A unit test for the loss ratio calculation exists and passes.
