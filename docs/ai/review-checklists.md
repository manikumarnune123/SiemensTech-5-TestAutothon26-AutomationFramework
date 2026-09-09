# Review Checklists

## Automation Review

- Requirement IDs are traceable to tests or documented fallback.
- Test method names describe business outcomes.
- Page objects hide locators and UI mechanics from tests.
- Test data and configuration are externalized.
- No production URLs, real credentials, private mobile numbers, personal data, or secrets are committed.
- Assertions verify business-visible outcomes: login success, selected location, product data, accepted offer, order placement, savings.
- Locators follow the hierarchy in [locator-strategy.md](locator-strategy.md).
- Synchronization uses Playwright/Appium waits and visible state checks, not sleeps.
- Screenshots, traces, videos, logs, and report steps are useful for debugging and judging.
- Failure and fallback paths are explicit.

## Test Strategy Review

- Product and user understanding is clear.
- Objectives map to revenue, trust, privacy, usability, conversion, and retention risks.
- Scope, out-of-scope, assumptions, dependencies, and limitations are explicit.
- Personas and critical journeys are represented.
- Functional, negative, boundary, cross-browser, mobile, accessibility, performance, security/privacy, and visual checks are covered.
- Entry/exit criteria and prioritization method are practical for a 5-hour challenge.
- AI-generated recommendations were reviewed and adapted to the actual site.

## Bug Report Review

- Defect is reproducible in staging.
- Steps are minimal and precise.
- Expected and actual results are objectively different.
- Severity and priority are justified by business or user impact.
- Evidence includes screenshot/video and console/network/system details where relevant.
- Report avoids sensitive data.
- Product feedback is separated from confirmed defects.

## AI Output Review

- AI output is validated against repo code, live site evidence, or challenge requirements.
- Hallucinated selectors, unavailable tools, invented facts, and unexecuted claims are removed.
- Prompts and incorporated outputs are recorded in [ai-usage-report.md](ai-usage-report.md).
- Any limitation or uncertainty is clearly noted for judges.

## Demo Readiness Review

- Repository builds cleanly.
- Intended smoke path can be started with documented commands.
- Reports open locally and show pass/fail summary.
- Important screenshots/videos are present.
- Fallback story is clear for blocked dependencies.
- README points judges to setup, execution, reports, strategy, bug report, AI usage, and limitations.
