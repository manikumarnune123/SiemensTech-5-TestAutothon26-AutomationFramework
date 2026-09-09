# AI Usage Report

Use this document as the source for the required TestAutothon AI disclosure. Keep entries concise, factual, and free of sensitive data.

## Summary

| Field | Details |
| --- | --- |
| Team | SiemensTech-5 |
| Repository | SiemensTech-5-TestAutothon26-AutomationFramework |
| AUT | `https://stg.gajab.com/` |
| Challenge | TestAutothon 2026 Automation Quest and Bug Quest |

## AI Tools And Models Used

| Tool/Model | Purpose | Notes |
| --- | --- | --- |
| GitHub Copilot Chat | Repo analysis, AI-native customization, documentation scaffolding, testing guidance. | Review and validate all generated content before submission. |

## Prompt And Output Log

| Time | Tool/Model | Prompt Or Activity | Output Used | Human Review/Validation | Limitations |
| --- | --- | --- | --- | --- | --- |
| 2026-09-09 | GitHub Copilot Chat | Create AI-native repo guidance from the TestAutothon requirement document for Gajab staging. | Copilot instructions, agents, prompts, requirements, strategy, review, and sprint memory scaffolding. | Repo structure and customization file formats reviewed; build/frontmatter validation required after changes. | Live site selectors and execution evidence still need validation against staging. |
| 2026-09-09 | GitHub Copilot Chat | Validate AI-native setup changes. | Diagnostics and diff hygiene results recorded in sprint memory. | VS Code diagnostics found no errors; `git diff --check` passed. Full `dotnet build` and project restore were attempted. | NuGet restore did not complete within the available command window, so compile validation remains pending. |

## Review Process

- Check every AI-generated requirement against the challenge document.
- Check every selector against the live staging DOM or Appium Inspector before relying on it.
- Run framework build and targeted tests after code changes.
- Remove invented results, screenshots, bugs, logs, or claims.
- Mask private mobile numbers, email addresses, credentials, tokens, personal data, and sensitive screenshots.

## Errors Or Hallucinations Found

| Date | Issue | Fix |
| --- | --- | --- |
| 2026-09-09 | No live Gajab DOM inspection has been performed in this AI setup step. | Treat locators and automation tasks as strategy until validated during implementation. |
| 2026-09-09 | NuGet restore did not complete during validation, leaving `project.assets.json` unavailable for a no-restore build. | Retry restore when network/package access is available, then run `dotnet build .\Testhon.slnx /p:InstallPlaywrightBrowsers=false`. |

## Time Saved Or Improvement

- Generated a reusable repo context pack for future AI sessions.
- Converted the challenge brief into traceable automation requirements and review checklists.
- Created reusable agents and prompts to reduce repeated context setup during the hackathon.
