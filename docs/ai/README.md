# AI-Native Workspace Guide

This folder is the shared context pack for building and reviewing the Gajab TestAutothon automation framework with AI assistance.

## Start Here

1. Read [gajab-requirements.md](gajab-requirements.md) for the challenge workflow and judging expectations.
2. Read [gajab-domain-knowledge.md](gajab-domain-knowledge.md) for product vocabulary, risks, personas, and business assertions.
3. Read [locator-strategy.md](locator-strategy.md) before creating or repairing selectors.
4. Update [sprint-memory.md](sprint-memory.md) after meaningful discoveries, decisions, validations, and blockers.
5. Record AI-assisted work in [ai-usage-report.md](ai-usage-report.md).

## Copilot Customizations

- Project-wide behavior: `.github/copilot-instructions.md`
- Task-specific instructions: `.github/instructions/*.instructions.md`
- Reusable slash prompts: `.github/prompts/*.prompt.md`
- Specialist agents: `.github/agents/*.agent.md`
- Pull request review guardrails: `.github/PULL_REQUEST_TEMPLATE.md`

## Operating Loop

1. Pick a requirement or risk from the strategy.
2. Ask the relevant agent for a focused plan or implementation.
3. Implement the smallest valuable change.
4. Run a targeted validation.
5. Save evidence and update sprint memory.
6. Review with the Gajab QA Reviewer before submission.

## Guardrails

- Use only the staging URL: `https://stg.gajab.com/`.
- Never commit secrets, credentials, private test accounts, personal data, API keys, or sensitive screenshots.
- Keep AI output explainable. The team must be able to defend every generated test, bug, and strategy statement to judges.
