---
description: "Generate or refresh a judge-ready Gajab TestAutothon strategy with traceable risk-based coverage and AI review notes."
name: "Gajab Test Strategy"
argument-hint: "Scope, requirement, new finding, risk, coverage gap, or strategy section"
agent: "Gajab Test Architect"
---

Create a patch-ready update to the Gajab TestAutothon strategy using the supplied input and current repository evidence. Preserve existing valid decisions; identify conflicts instead of silently overwriting them.

Use:

- [Gajab requirements](../../docs/ai/gajab-requirements.md)
- [Domain knowledge](../../docs/ai/gajab-domain-knowledge.md)
- [Current strategy](../../docs/testing/gajab-test-strategy.md)
- [Sprint memory](../../docs/ai/sprint-memory.md)
- [Review checklists](../../docs/ai/review-checklists.md)

Process:

- Map the input to one or more challenge requirements and label assumptions separately from observed facts.
- Prioritize by customer, revenue, trust, privacy, conversion, and retention impact; explain the ranking.
- Cover the required journey plus meaningful negative, boundary, recovery, and environment-fallback scenarios.
- Distinguish executed evidence from planned or configuration-only coverage across English/Hinglish, Chrome/Firefox/Edge, web/Android, and non-functional checks.
- Keep all test data authorized, synthetic, or explicitly event-provided; never reproduce secrets or personal data.

Return:

1. **Strategy delta**: the exact section(s) to add or replace.
2. **Traceability**: requirement IDs to risks, scenarios, test layers, and evidence.
3. **Risk table**: risk, likelihood, impact, priority, mitigation, and residual risk.
4. **Coverage matrix**: functional, negative/boundary, browser/device/language, accessibility, performance, security/privacy, and visual coverage with status.
5. **Execution gates**: entry criteria, exit criteria, stop conditions, fallback policy, and reporting artifacts.
6. **AI review note**: prompts used, recommendations accepted/rejected, human validation, limitations, and time/quality impact.

Do not claim a test was executed unless the repository or supplied evidence proves it.
