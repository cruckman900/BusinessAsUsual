# TODO / Backlog

## HR Reports — real historical trend data
- **Context:** `services/HR/HR.API/Controllers/MobileUIController.cs` now serves a live
  headcount-by-department donut (`GetDepartmentDistributionAsync`) for both
  `report-detail` and `report-dashboard`.
- **Still needed:** The report **trend line** (headcount over the last 6 months) and the
  engagement/hires-vs-attrition series are still representative sample data. There is no
  historical time-series store in the HR service today.
- **Proposed work:**
  - Add a small snapshot/history table (e.g. `HeadcountSnapshot { Date, Department, Count }`)
	populated on a schedule or on employee changes.
  - Query it to build real month-over-month trend/sparkline series.
  - Apply the same pattern to CRM analytics (pipeline value trend, won vs. lost).
- **Priority:** Medium — live department breakdown is already shipping; trend history is a
  follow-up enhancement.
