---
description: "Convert reproducible Gajab exploratory, automation, console, network, screenshot, or video evidence into judge-ready defect reports."
name: "Gajab Bug Report"
argument-hint: "Evidence bundle, failed scenario, logs, screenshot, video, or reproduction notes"
agent: "Gajab Bug Analyst"
---

Analyze the supplied Gajab evidence and create one or more judge-ready defect entries only when the evidence supports a reproducible product problem. Separate product defects from automation defects, environment outages, test-data gaps, and unsupported observations.

Use [bug report template](../../docs/testing/bug-report-template.md), [domain knowledge](../../docs/ai/gajab-domain-knowledge.md), and [review checklists](../../docs/ai/review-checklists.md).

Triage first:

- Verify the authorized AUT, timestamp, browser/device, language, account/data class, and preconditions.
- Check for duplicates, expected challenge behavior, transient infrastructure failure, and selector/framework faults.
- Require a concrete reproduction path and evidence reference; mark missing evidence as a blocker.
- Do not infer security impact without supporting console, network, or authorization evidence.
- Redact OTPs, credentials, private phone numbers, emails, tokens, and other sensitive data from the report.

Each accepted defect must include:

- Unique ID.
- Clear title.
- Module or feature.
- Environment.
- Preconditions.
- Reproduction steps.
- Expected result.
- Actual result.
- Severity and priority with rationale.
- Business or user impact.
- Reproducibility.
- Evidence references.
- Suggested resolution when useful.

Use this output order:

1. **Triage verdict**: valid defect, needs evidence, duplicate, environment issue, or automation issue.
2. **Defect entry**: all required fields, with severity/priority rationale and evidence links.
3. **Reproduction confidence**: exact repeat count, variants attempted, and observed consistency.
4. **Product feedback**: only when it is distinct from the defect and evidence-based.
5. **Open questions**: the smallest set of facts needed to finalize the report.

Reject or downgrade weak issues that are non-reproducible, unsupported by evidence, duplicate, purely cosmetic, outside the authorized environment, or prohibited by the challenge rules.
