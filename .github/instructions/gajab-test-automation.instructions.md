---
description: "Use when adding or changing Gajab .NET Playwright, NUnit, Appium, page object, test data, reporting, or framework automation code."
name: "Gajab Test Automation"
applyTo: ["tests/Testhon.Tests/**/*.cs", "src/Testhon.Framework/**/*.cs"]
---

# Gajab Test Automation

- Keep tests business-readable. A test should call page/component methods such as `LoginWithOtpAsync`, `SetPincodeAsync`, `FindMostBargainedTrendingProductAsync`, or `CompleteSandboxPaymentAsync` rather than exposing locator mechanics.
- Keep selectors in page objects or component objects, not in test methods.
- Prefer data-driven inputs from `TestData` and `RunSettings`; never hard-code real phone numbers, email addresses, credentials, or personal data.
- Use NUnit categories intentionally: `Smoke`, `Regression`, `E2E`, `Accessibility`, `Performance`, `Security`, `Visual`, `Mobile`, and `Fallback`.
- Capture artifacts for high-value steps: deal of the day, live order, cheapest Just Bargained product, selected product, accepted offer, order confirmation, and My Bargains savings.
- When a site dependency is unavailable, assert and report a documented fallback instead of silently skipping the business risk.
- Use `Retry` sparingly for known environmental flake; fix synchronization or locator issues first.
- Validate changes with the narrowest useful command, then update [docs/ai/sprint-memory.md](../../docs/ai/sprint-memory.md) with what was learned.
