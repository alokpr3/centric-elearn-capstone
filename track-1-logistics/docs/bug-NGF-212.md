# NGF-212: Dispatch on-time report understates performance

**Reported by:** Meredith Okafor (Operations Director)
**Component:** Reports - dispatch on-time
**Priority:** High - this report goes to the board
**Status:** Open

## Symptom

The September 2026 dispatch on-time report shows a network on-time rate of **75.5%** (157 of 208 consignments). My depot managers have counted the same month by hand from the booking board and get **84.9%**. They say 23 of the 208 consignments in that period were cancelled before they went out, and they do not think a cancelled consignment should count against us at all.

Leeds is the worst example. The report says **50.0%** (10 of 20). Leeds had 8 cancellations that month, almost all from one account customer (Calder Valley Retail). Without them the managers say Leeds was **83.3%** (10 of 12). Leeds has been getting phone calls from head office about a problem that is not really theirs.

## Steps to reproduce

1. Start the API.
2. Call `GET /api/reports/dispatch-on-time?from=2026-09-01&to=2026-09-30`.
3. Look at `total`, `onTime` and `onTimePct` for the network and for the Leeds row (depot code LDS).

Or open the Dispatch report page in the web app with the default dates.

## Expected

Cancelled consignments are not part of the rate. For September 2026 the network should show **157 on time out of 185, which is 84.9%**, and Leeds should show **10 out of 12, which is 83.3%**.

## Actual

Network: 157 out of 208, 75.5%. Leeds: 10 out of 20, 50.0%. The "late" column also grows by the number of cancellations, so the report says we were late on consignments we never moved.

## Notes

- A consignment counts as on time when it was delivered on or before its promised time.
- I have not checked the other depots in detail, but Sheffield also looks low.
