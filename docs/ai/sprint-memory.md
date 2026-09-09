# Sprint Memory

This is the living memory for the TestAutothon sprint. Update it whenever the team learns something that should guide future AI-assisted work.

## Current Sprint

- Date: 2026-09-09.
- Team/repo: SiemensTech-5-TestAutothon26-AutomationFramework.
- AUT: `https://stg.gajab.com/`.
- Framework: .NET 10, NUnit, Playwright, Appium, Serilog, ExtentReports.
- Mission: automate the Gajab bargain-to-order journey and produce strategy, bug report, execution report, and AI usage disclosure.

## Decisions

| Date | Decision | Rationale | Owner/Next Step |
| --- | --- | --- | --- |
| 2026-09-09 | Use repository-local Copilot customizations under `.github`. | Keeps AI guidance versioned and team-shared. | Maintain as requirements evolve. |
| 2026-09-09 | Treat Gajab staging as the default target URL. | Challenge explicitly forbids production testing. | Keep environment overrides non-production. |
| 2026-09-09 | Keep old SauceDemo tests explicit until replaced. | Prevents sample tests from running against Gajab by accident. | Build Gajab page objects and workflow tests. |
| 2026-09-09 | Copy CSV/XLSX test data templates to test output. | Supports data-driven challenge execution and Excel-compatible datasets. | Implement loader when Gajab tests are added. |
| 2026-09-09 | Standardize five repository-local Gajab prompts under `.github/prompts`. | Gives the team repeatable, role-specific workflows with explicit evidence, safety, and output contracts. | Use the prompts during implementation, strategy, defect triage, review, and AI disclosure. |

## Prompt Catalog

Use these prompts as focused, single-purpose entry points. Shared locator, domain, and automation rules remain in `.github/instructions` and should not be duplicated in prompt files.

| Prompt | Use When | Required Review |
| --- | --- | --- |
| [Gajab Automation Scenario](../../.github/prompts/gajab-automation-scenario.prompt.md) | Designing or implementing one Playwright/Appium business scenario. | Confirm requirement traceability, ID-first locator evidence, safe data, artifacts, and a narrow validation command. |
| [Gajab Test Strategy](../../.github/prompts/gajab-test-strategy.prompt.md) | Refreshing risk-based TestAutothon coverage or adding a strategy section. | Separate observed facts from assumptions and label executed, planned, partial, and blocked coverage. |
| [Gajab Bug Report](../../.github/prompts/gajab-bug-report.prompt.md) | Turning reproducible exploratory or automation evidence into a defect. | Triage product defect versus automation/environment issue; redact sensitive data and require evidence. |
| [Gajab Review](../../.github/prompts/gajab-review.prompt.md) | Reviewing code, reports, artifacts, or submission readiness. | Lead with severity-ordered findings and distinguish evidence-backed gaps from assumptions. |
| [Gajab AI Usage Report](../../.github/prompts/gajab-ai-usage-report.prompt.md) | Recording AI-assisted work for the mandatory disclosure. | Record only actual prompts, outputs, human validation, limitations, and defensible impact estimates. |

### Prompt Engineering Rules

- Keep each prompt focused on one task and require the smallest useful input.
- Read the applicable domain, locator, and automation instructions before acting.
- Prefer repository evidence over assumptions; never claim execution without proof.
- Use only the authorized staging AUT and redact secrets, credentials, personal data, and sensitive runtime output.
- Require a concrete output contract, a validation command, and an explicit fallback for unavailable dependencies.

## Backlog

| Priority | Item | Status | Notes |
| --- | --- | --- | --- |
| P0 | Create Gajab login and OTP page object. | Not started | Needs authorized mobile number injected securely. |
| P0 | Implement pincode selection and location assertion. | Not started | Start with pincode `560037` if valid. |
| P0 | Automate deal capture, trending analysis, live order evidence, and Just Bargained analysis. | Not started | Requires robust product card parsing. |
| P0 | Automate Toys & Games, brand, price range, product selection, bargaining, checkout, payment, order placed, and My Bargains savings. | Not started | Include fallback path for unavailable product/filter. |
| P1 | Add English and Hinglish language coverage. | Not started | Validate locators across both languages. |
| P1 | Add Chrome, Firefox, and Edge execution matrix. | Not started | Browser selection already exists in RunSettings. |
| P1 | Add Android mobile web and Appium-native execution notes. | Not started | APK and Appium locators need event setup. |
| P1 | Add accessibility, performance, security/privacy, and visual checks. | Not started | Keep checks safe and non-intrusive. |
| P1 | Prepare execution report sample and final known limitations. | Not started | Use TestResults artifacts. |
| P1 | Fill test strategy, bug report, and AI usage report. | In progress | Templates exist under docs. |

## Evidence Log

| Date | Evidence | Impact | Follow-Up |
| --- | --- | --- | --- |
| 2026-09-09 | Existing repo contains SauceDemo sample tests and page objects. | Good framework skeleton, but AUT-specific automation must be added. | Replace sample flows with Gajab page objects/tests. |
| 2026-09-09 | AI-native setup validation: editor diagnostics clean, `git diff --check` passed, but `dotnet restore` did not complete within 120 seconds and build could not generate `project.assets.json`. | Code/config edits appear syntactically clean; full compile validation is blocked by package restore in this environment. | Retry `dotnet restore .\tests\Testhon.Tests\Testhon.Tests.csproj --disable-parallel` when NuGet access is stable, then run build. |
| 2026-09-09 | Enhanced all five `.github/prompts/*.prompt.md` files with explicit inputs, decision gates, evidence rules, safety controls, and structured outputs; frontmatter diagnostics are clean. | Prompt use is more consistent and judge-facing claims are easier to audit. | Record actual prompt invocations and incorporated outputs in `docs/ai/ai-usage-report.md`. |

## Open Risks

- Authorized mobile number and team email are not stored in repo and must be injected securely.
- Product availability, brand filter, price range, and target dartboard product can change during the event.
- Payment sandbox flow may change provider pages or bank selection labels.
- Live Orders may expose dynamic data; assertions should avoid fixed names and capture evidence.
- Native Android locators require APK inspection with Appium Inspector.
- Language switching can affect visible text locators.

## Update Template

Append entries in this style:

```markdown
### YYYY-MM-DD HH:mm IST - Short Topic

- Observation:
- Decision:
- Validation:
- Evidence/artifacts:
- Follow-up:
```
