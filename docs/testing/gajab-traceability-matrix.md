# Gajab Requirement Traceability Matrix

Use this matrix to track automation, manual execution, evidence, and fallback status during the challenge.

| Requirement ID | Journey Step | Priority | Automation Status | Evidence/Artifact | Fallback/Notes |
| --- | --- | --- | --- | --- | --- |
| GJ-E2E-01 | Navigate to Gajab staging. | P0 | Not started |  |  |
| GJ-E2E-02 | Open Login or Sign up. | P0 | Not started |  |  |
| GJ-E2E-03 | Enter authorized mobile number and request OTP. | P0 | Not started |  | Mobile number must come from secure local config or environment variable. |
| GJ-E2E-04 | Enter OTP `123456` and verify login success. | P0 | Not started |  | Public challenge OTP only for staging. |
| GJ-E2E-05 | Enter pincode and verify reflected location. | P0 | Not started |  | Starter pincode: `560037` if valid at runtime. |
| GJ-E2E-06 | Capture Deal of the Day. | P0 | Not started |  | Capture image, name, and asking price. |
| GJ-E2E-07 | Email deal image, name, and asking price. | P1 | Not started |  | Use team email only; avoid private data in repo. |
| GJ-E2E-08 | Identify most-bargained Trending product with tie handling. | P0 | Not started |  | Choose first in scroll order when tied. |
| GJ-E2E-09 | Verify latest live order, name, city, screenshot. | P1 | Not started |  | Avoid exposing sensitive data in public evidence. |
| GJ-E2E-10 | View All in Just Bargained and identify cheapest among most-bargained products. | P0 | Not started |  | Requires card parsing and sorting logic. |
| GJ-E2E-11 | Open Toys & Games category. | P0 | Not started |  |  |
| GJ-E2E-12 | Select `SERA'S BASKET` brand filter. | P0 | Not started |  | Document fallback if brand is unavailable. |
| GJ-E2E-13 | Set price range INR 427 to INR 727. | P0 | Not started |  | Document fallback if slider or range values differ. |
| GJ-E2E-14 | Select target dartboard product. | P0 | Not started |  | Document fallback if product is unavailable. |
| GJ-E2E-15 | Open product and start bargaining. | P0 | Not started |  |  |
| GJ-E2E-16 | Bargain for 3 attempts and accept offer. | P0 | Not started |  | Capture accepted offer and attempts. |
| GJ-E2E-17 | Click Buy Now. | P0 | Not started |  |  |
| GJ-E2E-18 | Select Pay Online and click Pay. | P0 | Not started |  |  |
| GJ-E2E-19 | Choose Net banking and sandbox bank. | P0 | Not started |  | Any sandbox bank allowed. |
| GJ-E2E-20 | Click Success and confirm payment. | P0 | Not started |  | Sandbox only; no real credentials. |
| GJ-E2E-21 | Verify order placed. | P0 | Not started |  | Capture confirmation banner. |
| GJ-E2E-22 | Open My Bargains and verify savings. | P0 | Not started |  | Reconcile savings with accepted offer when possible. |

## Coverage Matrix

| Dimension | Required Coverage | Status | Notes |
| --- | --- | --- | --- |
| Language | English | Not started |  |
| Language | Hinglish | Not started | Validate text locators after switching. |
| Browser | Chrome/Chromium | Not started | Primary smoke target. |
| Browser | Firefox | Not started | Run once smoke is stable. |
| Browser | Edge | Not started | Browser enum/config supports Chromium-family execution. |
| Platform | Desktop web | Not started |  |
| Platform | Android mobile web | Not started | Use Playwright device emulation or real-device CDP. |
| Platform | Native Android | Not started | Requires APK and Appium locator discovery. |
| Non-functional | Accessibility | Not started | Safe, diagnostic checks only. |
| Non-functional | Performance | Not started | Single-user timings and network observations. |
| Non-functional | Security/privacy | Not started | No intrusive testing. |
| Non-functional | Visual comparison | Not started | Use stable checkpoints and screenshots. |
