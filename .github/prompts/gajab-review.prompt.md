---
description: "Perform a severity-ordered, evidence-based readiness review of Gajab automation, strategy, defects, AI usage, or demo artifacts."
name: "Gajab Review"
argument-hint: "Files, PR, report, test run, artifact bundle, or demo scenario"
agent: "Gajab QA Reviewer"
---

Review the supplied Gajab work product against a judge-facing quality bar. Treat the repository and supplied artifacts as the source of truth; do not assume unverified claims are complete.

Use:

- [Review checklists](../../docs/ai/review-checklists.md)
- [Gajab requirements](../../docs/ai/gajab-requirements.md)
- [Sprint memory](../../docs/ai/sprint-memory.md)

Review dimensions:

- Challenge traceability and end-to-end business-flow completeness.
- Page-object boundaries, data/config separation, ID-first locator evidence, synchronization, retries, and maintainability.
- Test-data safety, secret redaction, authorized staging usage, and explicit handling of environment gaps.
- Browser, language, platform, non-functional, reporting, artifact, CI/CD, and documentation coverage.
- Test validity: distinguish code defects, product defects, infrastructure failures, stale binaries, and missing execution evidence.

Output:

1. **Findings first**, ordered Critical, High, Medium, Low; each must include location, impact, evidence, and concrete fix.
2. **Coverage gaps** mapped to challenge requirements and labeled executed, partial, planned, or blocked.
3. **Validation plan** with the narrowest commands that would confirm each important finding.
4. **Residual risks and assumptions** including environment or data dependencies.
5. **Readiness verdict**: ready, conditionally ready, or not ready, with the top three release-blocking actions.

Do not praise or summarize unchanged work before reporting findings. Do not create findings without evidence or a clearly stated assumption.
