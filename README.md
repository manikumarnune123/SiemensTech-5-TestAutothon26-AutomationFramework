# Testhon — Playwright + .NET C# Automation Framework

A modular, production-ready UI automation framework built with **Playwright**, **.NET 10**, **NUnit**,
**Serilog** (structured logging) and **ExtentReports** (HTML reporting). Supports **desktop web**,
**Android mobile web** (device emulation) and **native mobile apps via Appium** (Android/iOS) from a
shared logging and reporting pipeline.

## AI-native TestAutothon workspace

This repository now includes a repo-local AI context pack for the Gajab TestAutothon challenge.
The authorized application under test is **Gajab staging**: `https://stg.gajab.com/`.

- Project-wide Copilot guidance: [.github/copilot-instructions.md](.github/copilot-instructions.md)
- Gajab requirements and domain context: [docs/ai/gajab-requirements.md](docs/ai/gajab-requirements.md), [docs/ai/gajab-domain-knowledge.md](docs/ai/gajab-domain-knowledge.md)
- Locator strategy: [docs/ai/locator-strategy.md](docs/ai/locator-strategy.md)
- Sprint memory: [docs/ai/sprint-memory.md](docs/ai/sprint-memory.md)
- Review checklists and PR guardrails: [docs/ai/review-checklists.md](docs/ai/review-checklists.md), [.github/PULL_REQUEST_TEMPLATE.md](.github/PULL_REQUEST_TEMPLATE.md)
- Test strategy and bug report templates: [docs/testing/gajab-test-strategy.md](docs/testing/gajab-test-strategy.md), [docs/testing/bug-report-template.md](docs/testing/bug-report-template.md)
- Reusable Copilot prompts: [.github/prompts](.github/prompts)
- Specialist testing agents: [.github/agents](.github/agents)

The original SauceDemo tests are retained as explicit framework examples only. Build the Gajab
page objects and workflow tests from the AI context pack before challenge execution.

## Project layout

```
Testhon/
├─ src/Testhon.Framework/        # Reusable framework (no tests here)
│  ├─ Configuration/             # FrameworkConfig + strongly-typed RunSettings (incl. AppiumSettings)
│  ├─ Driver/                    # ITestSession: PlaywrightDriver (web) + AppiumSession (mobile)
│  ├─ Elements/                  # ElementActions (web) + AppiumElementActions/MobileLocator (mobile)
│  ├─ Enums/                     # BrowserEngine, Platform, CaptureMode, SwipeDirection
│  ├─ Logging/                   # Serilog setup
│  ├─ Pages/                     # BasePage (web) + MobilePage (mobile)
│  └─ Reporting/                 # ExtentReports manager + Serilog→Extent sink
└─ tests/Testhon.Tests/          # Test project
   ├─ Base/                      # BaseTest (web) + MobileBaseTest (Appium)
   ├─ Setup/GlobalSetup.cs       # One-time logging + report bootstrap
   ├─ Pages/                     # Sample page objects (web + Pages/Mobile for Appium)
   ├─ Tests/                     # Sample tests (web + mobile)
   └─ appsettings*.json          # Environment-based configuration
```

## Prerequisites

```powershell
dotnet build
# Install Playwright browsers once (path uses the built TFM):
pwsh tests/Testhon.Tests/bin/Debug/net10.0/playwright.ps1 install
```

## Run — Desktop

```powershell
$env:TEST_ENV="QA"
dotnet test
```

The default QA/UAT base URL is Gajab staging. Override with `RunSettings__BaseUrl` only for
authorized non-production environments.

## Run — Android mobile web (device emulation)

```powershell
$env:RunSettings__Platform="Android"
$env:RunSettings__DeviceName="Pixel 5"
dotnet test
```

## Run — Real Android device (Chrome via ADB + CDP)

The .NET Playwright binding has no native Android API, so real devices are driven through the
Chrome DevTools Protocol tunneled over ADB.

1. Enable **Developer options → USB debugging** on the phone and connect it.
2. Verify it is visible: `adb devices`.
3. Open **Chrome** on the device at least once.
4. Run (the framework runs `adb forward` automatically):

```powershell
$env:RunSettings__Platform="RealAndroid"
$env:RunSettings__AndroidCdpEndpoint="http://localhost:9222"
dotnet test
```

Notes: trace/video are disabled for real devices (screenshots on failure still work); `adb` must be
on `PATH` (part of Android Platform Tools).

## Run — Native mobile app (Appium)

Native Android/iOS apps are driven with **Appium** through the mobile test stack (`MobileBaseTest`,
`MobilePage`, `AppiumElementActions`, `MobileLocator`), reusing the same logging and HTML reporting as
the web tests. Screenshots are captured on failure and screen recording is saved per the `Video`
setting.

Prerequisites:

1. Install and start an **Appium** server (`npm i -g appium`, then `appium`) with the **UiAutomator2**
   driver (`appium driver install uiautomator2`).
2. Have an Android emulator/device online (`adb devices`).
3. Drop the app package at `tests/Testhon.Tests/TestData/gajab.apk` (it is copied to the test output
   automatically), or point `RunSettings:Appium:App` at any `.apk`/`.ipa` path.

Configure under `RunSettings:Appium` in `appsettings.json` (or via `RunSettings__Appium__*` env vars):

```powershell
$env:RunSettings__Appium__App="D:\builds\gajab.apk"
$env:RunSettings__Appium__DeviceName="Android Emulator"
$env:RunSettings__Appium__ServerUrl="http://127.0.0.1:4723"
dotnet test --filter "TestCategory=Mobile"
```

Mobile-web variant (drive Chrome on the device to the site instead of a native app): set
`RunSettings:Appium:BrowserName` to `Chrome` and leave `App` empty; the session opens
`RunSettings:Appium:BaseUrl` (default `https://stg.gajab.com/`).

The sample mobile tests (`MobileLoginTests`) are marked `[Explicit]` so they only run when selected
(`--filter "TestCategory=Mobile"`) — they need a live Appium server and app. Their page-object
locators in `tests/Testhon.Tests/Pages/Mobile` are placeholders: replace them with the real
accessibility-ids / resource-ids from the APK (inspect with **Appium Inspector**).

Any setting can be overridden by environment variables using the `RunSettings__Key` convention,
or per environment via `appsettings.{TEST_ENV}.json`.

## Run in Visual Studio

- Open `Testhon.sln`, build once — Playwright browsers install automatically (one-time MSBuild target).
- Use **Test Explorer** to run/debug; `test.runsettings` is wired via `<RunSettingsFilePath>`.
- To target Android emulation/real device in VS, edit `Platform` in `tests/Testhon.Tests/appsettings.json`.

## Reports & logs (under `tests/Testhon.Tests/TestResults/`)

All run outputs are written to the test project's `TestResults/` folder (override the location with
the `TESTHON_RESULTS_DIR` environment variable).

- **Testhon HTML report** (self-contained dashboard: donut, clickable KPI filters, searchable test
  cards, per-step pass/fail table, embedded failure screenshots, artifact links, dark/light theme):
  `TestResults/testhon-report.html`
- ExtentReports (Spark) HTML report: `TestResults/index.html`
- Screenshots on failure: `TestResults/screenshots/`
- Trace (view with `pwsh playwright.ps1 show-trace <zip>`): `TestResults/traces/`
- Video on failure: `TestResults/videos/`
- Rolling logs: `TestResults/logs/testrun-*.log`
- XML/TRX: `dotnet test --logger "trx" --logger "nunit;LogFilePath=TestResults/results.xml"`

## Run a subset

```powershell
dotnet test --filter "Category=Smoke"
```
