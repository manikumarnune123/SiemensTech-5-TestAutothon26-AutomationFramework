---
description: "Use when creating, reviewing, healing, or refactoring Playwright or Appium locators for Gajab web, mobile web, or native Android flows."
name: "Gajab Locator Strategy"
applyTo: ["tests/Testhon.Tests/Pages/**/*.cs", "src/Testhon.Framework/Elements/**/*.cs", "tests/Testhon.Tests/Tests/**/*.cs"]
---

# Gajab Locator Strategy

- Follow the locator hierarchy in [docs/ai/locator-strategy.md](../../docs/ai/locator-strategy.md).
- Prefer role, label, placeholder, alt text, accessible name, stable data attributes, and scoped section locators before CSS structure.
- Avoid generated class names, absolute XPath, text-only selectors for repeated cards, fixed indexes, animation-timing assumptions, and arbitrary sleeps.
- For product cards, anchor on product name or section, then scope to the card before reading price, bargains, image, or action buttons.
- For filters, prefer accessible checkbox labels or stable values. Verify the selected filter is reflected in the result state.
- For Appium, prefer accessibility ID, resource ID, and stable text; XPath is a last resort and must be documented as locator debt.
- Every brittle fallback locator should include a note in sprint memory with evidence and a follow-up to request a stable test id.
