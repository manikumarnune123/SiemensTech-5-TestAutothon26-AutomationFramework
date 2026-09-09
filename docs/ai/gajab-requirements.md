# Gajab TestAutothon Requirements

## Application Under Test

- Product: Gajab India's Bargain Bazar.
- URL: `https://stg.gajab.com/`.
- Do not use production for hackathon testing.
- Default challenge OTP: `123456` for the staging OTP flow.

## Required Deliverables

- Automation framework source code and execution report through the GitHub repository.
- Test strategy document.
- Bug report with evidence.
- AI usage disclosure covering tools, prompts, generated outputs used, validation, limitations, and time saved.

## End-To-End Workflow To Automate

| ID | Requirement |
| --- | --- |
| GJ-E2E-01 | Navigate to Gajab staging. |
| GJ-E2E-02 | Click Login or Sign up. |
| GJ-E2E-03 | Enter an authorized mobile number and request OTP. |
| GJ-E2E-04 | Enter default OTP `123456`, submit, and verify successful login. |
| GJ-E2E-05 | Enter pincode and verify selected location is reflected. |
| GJ-E2E-06 | Capture Gajab Deal of the Day. |
| GJ-E2E-07 | Email product image, product name, and asking price to the team email address. |
| GJ-E2E-08 | Identify the most-bargained product under Trending Products. If tied, use the first product in scroll order. |
| GJ-E2E-09 | Verify latest live order, capture the buyer name and city, and save a screenshot. |
| GJ-E2E-10 | Open View All in Just Bargained Product and identify the cheapest product among most-bargained products. |
| GJ-E2E-11 | Open Toys & Games category. |
| GJ-E2E-12 | Select brand `SERA'S BASKET` from the left filter. |
| GJ-E2E-13 | Set price range from INR 427 to INR 727. |
| GJ-E2E-14 | Select `Classic 15.7 Inch Soft Tip Dartboard Game Set`. |
| GJ-E2E-15 | Open product and start bargaining. |
| GJ-E2E-16 | Bargain for 3 attempts and accept the offer. |
| GJ-E2E-17 | Click Buy Now. |
| GJ-E2E-18 | Select payment method `Pay Online` and click Pay. |
| GJ-E2E-19 | Choose Net banking and select any sandbox bank. |
| GJ-E2E-20 | Click Success on the sandbox bank page and confirm payment. |
| GJ-E2E-21 | Verify order placed confirmation. |
| GJ-E2E-22 | Open My Bargains and verify savings. |

## Expected Framework Capabilities

- Modular framework with clear separation of tests, page objects, data, configuration, reporting, and reusable drivers.
- Parameterized scenarios and data-driven execution.
- Test data sourced from external files. Prefer Excel for judged data and keep repository templates free of private data.
- Positive and negative datasets.
- Secure handling of OTPs, credentials, personal data, environment values, and screenshots.
- Robust exception handling, recovery, screenshots, traces, videos, logs, and pass/fail summaries.
- CI/CD-ready commands and report artifacts.

## Coverage Expectations

- Languages: English and Hinglish.
- Browsers: Chrome, Firefox, and Edge.
- Platforms: desktop web, Android mobile web, and native Android executable approach through Appium.
- Non-functional quality: accessibility, performance, security/privacy, and visual comparison.

## Fallback Protocol

If a product, price, filter, OTP, payment, or data dependency is unavailable:

1. Capture screenshot and logs.
2. Record timestamp, browser, platform, URL, test data, and impacted requirement ID.
3. Explain why the dependency blocked the planned path.
4. Demonstrate a reasonable fallback, such as selecting the next matching product, documenting filter unavailability, or validating the journey up to the blocked step.
5. Add the limitation to sprint memory, report output, and final known limitations.

## Prohibited Activity

- Production testing.
- Denial-of-service or uncontrolled load testing.
- Data extraction, scraping, privilege escalation, or intrusive security testing.
- Unauthorized accounts, mobile numbers, email addresses, devices, or payment methods.
- Public disclosure of secrets, personal data, credentials, OTPs, private screenshots, or API keys.
