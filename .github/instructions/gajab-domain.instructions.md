---
description: "Use when testing Gajab, writing domain knowledge, creating TestAutothon strategy, bug reports, user journeys, or product feedback for the Gajab staging site."
name: "Gajab Domain Context"
---

# Gajab Domain Context

- Authorized AUT: `https://stg.gajab.com/` only.
- Product: bargain-led Indian ecommerce marketplace where users log in with mobile OTP, set location, bargain with sellers, pay, and review savings under My Bargains.
- Challenge journey: login or sign up, OTP `123456`, pincode selection, deal capture and email, trending product analysis, live order verification, Just Bargained analysis, Toys & Games category filtering, bargain attempts, payment, order confirmation, and savings verification.
- Required language coverage: English and Hinglish.
- Required browser coverage: Chrome, Firefox, and Edge.
- Required platform coverage: web and Android. Native Android uses Appium; mobile web can use Playwright device emulation or real device CDP.
- Non-functional checks should be relevant and evidence based: accessibility, performance, security/privacy, and visual comparison.
- Document genuine environment gaps with screenshots, logs, timestamp, impacted step, fallback path, and judge-facing explanation.

Source context lives in [docs/ai/gajab-requirements.md](../../docs/ai/gajab-requirements.md) and [docs/ai/gajab-domain-knowledge.md](../../docs/ai/gajab-domain-knowledge.md).
