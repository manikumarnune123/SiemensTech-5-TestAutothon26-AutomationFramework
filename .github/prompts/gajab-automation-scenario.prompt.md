---
description: "Design or implement one evidence-backed Gajab Playwright or Appium scenario from a workflow step, risk, bug, or requirement."
name: "Gajab Automation Scenario"
argument-hint: "Workflow step, requirement, risk, failure evidence, or page to automate"
agent: "Gajab Automation Engineer"
---

Design or implement one maintainable Gajab automation scenario from the supplied input. Keep this prompt focused on one business behavior; do not invent unrelated coverage.

Read first:

- [Gajab requirements](../../docs/ai/gajab-requirements.md)
- [Domain knowledge](../../docs/ai/gajab-domain-knowledge.md)
- [Locator strategy](../../docs/ai/locator-strategy.md)
- [Sprint memory](../../docs/ai/sprint-memory.md)
- Applicable instructions in [Gajab domain context](../instructions/gajab-domain.instructions.md), [locator strategy](../instructions/gajab-locator-strategy.instructions.md), and [test automation](../instructions/gajab-test-automation.instructions.md)

Before editing:

- Identify the requirement or challenge step being covered.
- Inspect the nearest existing page object, test, test data, and evidence source.
- State one falsifiable implementation hypothesis and the narrowest validation command.
- Confirm the scenario uses only the authorized staging AUT and safe, non-secret data.

Implementation rules:

- Keep selectors in page/component objects and prefer the locator hierarchy from the locator strategy.
- Prefer stable IDs when the supplied DOM proves they exist; otherwise use role, label, placeholder, accessible name, or a scoped fallback.
- Keep tests business-readable and data-driven through `TestData` or `RunSettings`.
- Capture the required artifact for high-value steps and preserve evidence paths in the report.
- If a dependency is unavailable, fail with a documented fallback outcome rather than silently skipping the risk.
- Do not expose secrets, real credentials, private contact data, or sensitive runtime output.

Return and implement when possible:

1. **Decision**: objective, requirement IDs, scope, assumptions, and known dependency risks.
2. **Design**: test data, page/component responsibilities, locator evidence, synchronization, and artifacts.
3. **Changes**: files changed, NUnit test name/categories, and fallback behavior.
4. **Validation**: exact narrow command, result, and any environment blocker.
5. **Follow-up**: sprint-memory entry containing selector evidence, residual risk, and requested AUT improvements.
