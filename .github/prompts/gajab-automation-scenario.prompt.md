---
description: "Create or update a Gajab automation scenario from a workflow step, risk, bug, or requirement."
name: "Gajab Automation Scenario"
argument-hint: "Workflow step, scenario, or risk to automate"
agent: "Gajab Automation Engineer"
---

Create or update an automation scenario for Gajab using the provided input.

Read first:

- [Gajab requirements](../../docs/ai/gajab-requirements.md)
- [Domain knowledge](../../docs/ai/gajab-domain-knowledge.md)
- [Locator strategy](../../docs/ai/locator-strategy.md)
- [Sprint memory](../../docs/ai/sprint-memory.md)

Output and implement when possible:

- Scenario objective and linked requirement IDs.
- Test data needed and whether it is safe placeholder data or authorized event data.
- Page object/component changes.
- NUnit test method with categories.
- Artifacts to capture.
- Validation command to run.
- Sprint memory update summarizing decision, selectors, and remaining risk.
