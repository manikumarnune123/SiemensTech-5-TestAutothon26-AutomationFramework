# Gajab Locator Strategy

## Goal

Selectors should survive dynamic product data, responsive layouts, language changes, and minor styling changes. A locator is good when a teammate can explain why it represents the user-visible intent.

## Locator Hierarchy

1. Stable test IDs requested from the product team, such as `data-testid`, `data-test`, or `data-qa`.
2. Accessibility locators: role, accessible name, label, placeholder, alt text, and title.
3. Text locators scoped to a section, card, dialog, or form.
4. Stable semantic attributes such as `href`, `name`, `type`, `aria-*`, or known form values.
5. Scoped CSS using meaningful parent containers.
6. XPath only as a documented last resort.

## Playwright Patterns

- Prefer scoped locators in page objects: section first, card second, field or action third.
- Product cards should be found by visible product name or by card-level data attribute. Then read nested price, bargain count, image, and buttons from that card.
- For repeated sections such as Trending or Just Bargained, first locate the section heading, then search inside the section.
- For modal/dialog flows such as login, OTP, bargaining, and payment, scope actions inside the active dialog when possible.
- For dynamic order strips, assert the required fields exist and capture evidence instead of relying on a fixed position unless the requirement explicitly needs latest or first item.

## Appium Patterns

- Prefer accessibility ID and resource ID.
- Use visible text only when stable across the selected language.
- Keep Android package/activity and app path in configuration.
- XPath must include a sprint memory note explaining why stronger locators were unavailable.

## Anti-Patterns

- Generated class names.
- Absolute XPath.
- `nth-child` selectors for business data.
- Unscoped text selectors where the same text appears in multiple cards.
- Hard-coded animation delays or sleeps.
- Selectors that work only in one language when the scenario must support English and Hinglish.

## Self-Healing Notes

When a locator breaks:

1. Capture screenshot, trace, DOM snippet, current language, viewport, browser, and timestamp.
2. Determine whether the failure is data absence, layout shift, language change, delayed loading, or selector drift.
3. Repair the locator using the hierarchy above.
4. Add a fallback only when it represents the same user intent.
5. Update sprint memory with the old locator, new locator, validation, and remaining risk.

## Suggested Stable Test IDs To Request

- `login-open-button`
- `mobile-number-input`
- `request-otp-button`
- `otp-digit-input`
- `otp-submit-button`
- `location-pincode-input`
- `deal-of-day-card`
- `trending-product-card`
- `live-order-item`
- `just-bargained-card`
- `category-toys-games-tab`
- `brand-filter-seras-basket`
- `price-range-filter`
- `bargain-offer-slider`
- `accept-offer-button`
- `buy-now-button`
- `payment-method-pay-online`
- `payment-netbanking-option`
- `payment-success-button`
- `order-confirmation-banner`
- `my-bargains-savings`
