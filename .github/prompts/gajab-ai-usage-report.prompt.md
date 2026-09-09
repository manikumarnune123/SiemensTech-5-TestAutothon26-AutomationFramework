---
description: "Maintain a transparent, evidence-backed TestAutothon AI usage report covering prompts, outputs, validation, limitations, and impact."
name: "Gajab AI Usage Report"
argument-hint: "AI-assisted activity, prompt sequence, generated change, validation, or limitation"
agent: "Gajab QA Reviewer"
---

Update [docs/ai/ai-usage-report.md](../../docs/ai/ai-usage-report.md) from the supplied activity. Record only work that actually occurred in this repository or conversation; do not fabricate model usage, prompts, validation, time savings, or outcomes.

Capture:

- Tool or model used.
- Task supported.
- Important prompt or prompt sequence.
- Output incorporated into the submission.
- Human review and validation performed.
- Errors, hallucinations, or limitations found.
- Time saved or quality improvement.
- Sensitive-data checks performed.

For each activity, capture:

- Date or phase, tool/model, task, prompt sequence, repository context supplied, and output incorporated.
- Human review decisions, executable validation, files changed, and whether the result was accepted, modified, or rejected.
- Errors, hallucinations, stale assumptions, environment limitations, and remaining risk.
- Time saved or quality improvement as an estimate labeled with its basis, not as an unsupported metric.

Apply these controls:

- Redact secrets, credentials, tokens, private phone numbers, personal data, proprietary content, and non-public URLs.
- Do not reproduce the real OTP or sensitive test data; refer to authorized challenge defaults generically.
- Separate AI-generated suggestions from human-authored decisions and observed runtime facts.
- Preserve an auditable link to the relevant source file, test result, diff, or artifact where available.

Return a concise patch to the report followed by an omissions list for facts the user must supply. Do not create a second report file.
