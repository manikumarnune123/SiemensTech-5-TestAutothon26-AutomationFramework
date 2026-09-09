# Project Guidelines

## Mission

This repository is the TestAutothon 2026 automation and quality workspace for Gajab India's Bargain Bazar. Treat `https://stg.gajab.com/` as the only authorized application under test unless the user explicitly provides another non-production URL.

Before generating or changing tests, read the AI context pack:

- [docs/ai/gajab-requirements.md](../docs/ai/gajab-requirements.md)
- [docs/ai/gajab-domain-knowledge.md](../docs/ai/gajab-domain-knowledge.md)
- [docs/ai/locator-strategy.md](../docs/ai/locator-strategy.md)
- [docs/ai/sprint-memory.md](../docs/ai/sprint-memory.md)

## Architecture

- Keep reusable framework code in `src/Testhon.Framework`; keep challenge tests, page objects, and data under `tests/Testhon.Tests`.
- Use page objects for Gajab screens and components. Test methods should read like business flows and avoid raw locator details.
- Use `RunSettings` and `appsettings*.json` for environment, browser, platform, device, timeout, artifact, and Appium configuration.
- Route web actions through `IElementActions` when a string selector is sufficient. Use direct Playwright locators in page objects only when semantic or scoped locators are needed for resilience.
- Keep Appium/native mobile locators in mobile page objects and prefer accessibility IDs or resource IDs over XPath.

## Build And Test

Use PowerShell-friendly commands on Windows:

```powershell
dotnet build .\Testhon.slnx /p:InstallPlaywrightBrowsers=false
$env:TEST_ENV="QA"; dotnet test .\Testhon.slnx --filter "Category=Smoke"
```

Install Playwright browsers when needed:

```powershell
pwsh tests/Testhon.Tests/bin/Debug/net10.0/playwright.ps1 install
```

## AI-Native Workflow

- Update [docs/ai/sprint-memory.md](../docs/ai/sprint-memory.md) whenever you make a meaningful decision, discover a risk, validate a selector, or choose a fallback.
- Record useful prompts, AI output incorporated, review steps, and limitations in [docs/ai/ai-usage-report.md](../docs/ai/ai-usage-report.md).
- Use the custom agents in `.github/agents` for focused work: test architecture, automation implementation, locator strategy, QA review, and bug analysis.
- Use the prompts in `.github/prompts` for repeatable outputs such as scenarios, bug reports, review passes, test strategy updates, and AI usage reports.

## Safety And Submission Rules

- Do not test production Gajab URLs.
- Do not perform denial-of-service, uncontrolled load, privilege escalation, scraping, or intrusive security testing.
- Do not commit real OTPs, credentials, private phone numbers, personal data, API keys, screenshots with sensitive data, or secrets.
- Use only authorized test accounts, mobile numbers, email addresses, devices, and payment sandbox flows.
- If a product, filter, price, OTP flow, payment flow, or other dependency is unavailable, document evidence and a reasonable fallback instead of hiding the failure.
