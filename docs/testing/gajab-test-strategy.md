# SiemensTech-5_TestAutothon26_TestStrategy

## Product And User Understanding

Gajab India's Bargain Bazar is a bargain-led ecommerce experience. The critical user promise is that a shopper can log in, discover a product, negotiate a better price, pay successfully, and later verify savings in My Bargains. Testing must cover both transactional correctness and user trust in pricing, savings, payment, and privacy.

## Objectives

- Validate the end-to-end bargain-to-order workflow on staging.
- Verify that dynamic merchandising areas expose correct and useful product information.
- Identify defects that affect revenue, conversion, trust, privacy, accessibility, or mobile usability.
- Demonstrate a maintainable automation framework with useful reports, logs, and artifacts.
- Provide a clear fallback story for unavailable products, filters, or sandbox dependencies.

## Scope

- Mobile OTP login on staging.
- Pincode selection.
- Deal of the Day capture and email step.
- Trending, Live Orders, and Just Bargained analysis.
- Toys & Games category, brand filter, price range, product selection.
- Bargain attempts, accepted offer, checkout, online payment sandbox, order confirmation, My Bargains savings.
- English and Hinglish language checks.
- Chrome, Firefox, Edge, mobile web, and Android executable approach.
- Accessibility, performance, security/privacy, and visual checks with safe scope.

## Out Of Scope

- Production testing.
- Uncontrolled load, denial-of-service, scraping, privilege escalation, or intrusive security testing.
- Real payments or unauthorized accounts.
- Backend data mutation outside the user journey.

## Assumptions And Dependencies

- Staging URL is available during the event.
- Default OTP `123456` works for authorized challenge numbers.
- Target product, category, brand filter, and price range remain available or can be handled through documented fallback.
- Payment uses a sandbox flow with a success option.
- APK and Appium environment are available for native Android demonstration or execution.

## Risk Assessment

| Risk | Impact | Likelihood | Priority | Mitigation |
| --- | --- | --- | --- | --- |
| OTP/login failure | Blocks all personalized workflows. | Medium | P0 | Validate early; keep fallback evidence. |
| Dynamic product data changes | Breaks product-specific automation. | High | P0 | Use data-driven search and fallback product logic. |
| Bargain state changes unpredictably | Incorrect accepted offer or savings validation. | Medium | P0 | Assert state transitions and capture artifacts. |
| Payment sandbox changes | Blocks order completion. | Medium | P0 | Isolate payment page object and document fallback. |
| Weak locators | Flaky test execution. | High | P0 | Follow locator strategy and update sprint memory. |
| Privacy leak in reports | Submission risk. | Medium | P0 | Mask sensitive data and review artifacts. |
| Mobile layout differences | Missed conversion issues. | Medium | P1 | Run mobile viewport/Appium checks. |
| Accessibility gaps | User exclusion and judge impact. | Medium | P1 | Add focused keyboard, label, contrast, and screen reader checks. |

## Personas And Critical Journeys

- Bargain seeker: logs in, negotiates a lower price, pays, verifies savings.
- New user: completes OTP sign-up without confusion.
- Mobile shopper: performs the same journey on small screens or Android.
- Reviewer/judge: inspects artifacts and sees explainable automation, defects, and AI usage.

## Functional Scenarios

- Successful OTP login.
- Invalid or incomplete mobile number handling.
- Pincode selection and reflected location.
- Deal of the Day data extraction.
- Trending product max bargain count and tie handling.
- Latest live order name/city capture.
- Just Bargained cheapest product among most-bargained products.
- Toys & Games filter by brand and price range.
- Target product details page opens from filtered result.
- Three bargain attempts and accepted offer.
- Buy Now checkout with Pay Online.
- Net banking sandbox success.
- Order placed confirmation.
- My Bargains savings verification.

## Negative And Boundary Scenarios

- Empty, short, alphabetic, or unauthorized mobile number.
- Incorrect OTP and resend countdown behavior.
- Invalid or unsupported pincode.
- No products after filter combination.
- Price boundary exactly INR 427 and INR 727.
- Bargain amount below/above allowed range.
- Payment failure or back navigation from sandbox.
- Session timeout before checkout.
- Language switch mid-flow.

## Cross-Browser And Device Coverage

- Smoke E2E on Chromium first.
- Browser matrix on Chrome, Firefox, and Edge for critical flow.
- Mobile web with configured device emulation.
- Native Android with Appium if APK and locators are available; otherwise provide executable setup and limitation.

## Accessibility Considerations

- Login, OTP, filters, bargain controls, and payment choices are keyboard reachable.
- Inputs have labels or accessible names.
- Dynamic updates announce meaningful state or remain discoverable.
- Color contrast is sufficient for price, discount, and error states.
- Product images have useful alternative text or equivalent product name nearby.

## Performance Considerations

- Capture page load and key transition timings for home, category, product detail, bargain modal, checkout, and confirmation.
- Watch for excessive network failures, slow images, script errors, and repeated API retries.
- Avoid load tests; keep checks single-user and diagnostic.

## Security And Privacy Considerations

- No production testing.
- No secrets or personal data in repo, prompts, logs, screenshots, or reports.
- Verify OTP and session flows do not expose sensitive information in UI, console, or network logs.
- Validate payment sandbox does not request real credentials.
- Avoid intrusive security activity.

## Test Data Requirements

- Authorized mobile number supplied through environment variable or secure local config.
- Team email supplied through environment variable or secure local config.
- Default challenge OTP `123456`.
- Pincode, language, browser, platform, product target, brand, price range, and fallback product strategy.
- Maintain Excel-compatible data outside public secrets. A CSV starter template is available at `tests/Testhon.Tests/TestData/gajab-test-data.template.csv`.

## Entry Criteria

- Staging site reachable.
- Authorized mobile/email test data available.
- Browser dependencies installed.
- Appium/Android setup ready if mobile native is in scope.
- Test data and environment values configured.

## Exit Criteria

- P0 automated workflow executed or fallback documented with evidence.
- Execution report generated with screenshots/logs/traces/videos as applicable.
- Test strategy and bug report reviewed.
- AI usage report completed.
- Known limitations listed.
- No sensitive data in repository or submission artifacts.

## Execution Approach

- Start with Chromium desktop smoke path.
- Add page objects and component objects around stable user journeys.
- Expand to browser matrix and mobile coverage after smoke stability.
- Run exploratory testing around high-risk areas and convert valid defects into reports.
- Use AI agents/prompts for scenario generation, locator review, bug report drafting, and final review, then validate outputs manually.

## Prioritization Method

Prioritize by business impact, likelihood, challenge requirement value, automation feasibility, and evidence quality. P0 covers blockers to login, bargain, checkout, payment, order confirmation, and savings. P1 covers matrix, non-functional checks, and strong bug evidence. P2 covers polish and extended product feedback.

## Known Limitations

- Live Gajab selectors are not yet validated in this repository setup step.
- Existing SauceDemo sample tests are kept explicit until replaced by Gajab tests.
- Native Android execution depends on APK availability and Appium Inspector locator discovery.
- Inventory and pricing may change during the event.

## AI Usage Notes

AI is used to accelerate strategy, documentation, scenario design, locator review, implementation scaffolding, bug report drafting, and review. Every AI-generated output must be checked against the challenge requirements, live site evidence, and actual framework behavior before submission.
