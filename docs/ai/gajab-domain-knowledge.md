# Gajab Domain Knowledge

## Product Model

Gajab is a bargain-led ecommerce experience. The core value proposition is not only product discovery and checkout, but also the feeling that the customer negotiated a better price and can verify savings afterward.

## Core Concepts

| Concept | Meaning | Test Importance |
| --- | --- | --- |
| Mobile OTP login | User identity is established through mobile number and OTP. | Entry point for every personalized flow; high privacy and conversion risk. |
| Pincode/location | Customer selects delivery area. | Can affect product availability, delivery promise, and trust. |
| Deal of the Day | Highlighted bargain opportunity. | Important merchandising surface; must show product image, name, and asking price. |
| Trending Products | Products with bargain counts. | Requires ordering logic and tie handling. |
| Live Orders | Social proof strip showing recent orders. | Must avoid exposing sensitive data while staying credible. |
| Just Bargained | Products recently negotiated by users. | Used to identify cheapest product among highly bargained products. |
| Bargain attempts | User makes offers and receives seller responses. | Critical differentiator; needs stable state handling across attempts. |
| Accepted offer | Seller or system offer the user accepts. | Drives final payable price and savings. |
| Pay Online | Online payment method routed through sandbox payment. | Revenue-critical checkout risk. |
| My Bargains | User area showing negotiated products and savings. | Final proof that negotiated value persisted. |

## Primary Personas

- Bargain seeker: price-sensitive shopper trying to lower product cost.
- New user: signs up quickly with mobile OTP and expects frictionless onboarding.
- Mobile shopper: uses smaller viewport or Android app and expects same bargain journey.
- Judge/reviewer: wants evidence that automation is robust, maintainable, and explainable.

## High-Risk Journeys

- OTP login and privacy-safe handling of mobile number.
- Pincode selection and location reflection.
- Dynamic product cards with changing inventory, price, and bargain counts.
- Bargain attempt state machine and accepted offer price calculation.
- Payment sandbox handoff and success return.
- Order confirmation and My Bargains savings persistence.

## Business Assertions Worth Automating

- Login success message appears after OTP submission.
- Selected pincode/location remains visible or affects the user context.
- Deal of the Day exposes image, name, and asking price.
- Most-bargained product is selected by maximum bargain count, with first-in-scroll tie handling.
- Latest live order shows name and city without revealing excessive personal details.
- Cheapest product among most-bargained Just Bargained results is correctly identified.
- Product filters narrow results to expected brand and price range.
- Bargain attempts count and offer states are deterministic enough for validation.
- Accepted offer becomes the checkout price.
- Payment success returns to order confirmation.
- My Bargains savings match or reasonably reconcile with accepted offer and MRP.

## Test Data Notes

- Store authorized event mobile numbers and email addresses outside public commits or inject them through environment variables.
- The default challenge OTP `123456` is public test data for staging, but avoid normalizing OTP disclosure beyond this challenge context.
- Pincode `560037` appears in the brief screenshots and can be used as a starter value if valid during execution.
- Keep a fallback product strategy because inventory and filters may change during the event.

## Product Feedback Themes

- Clarity of bargain rules and remaining attempts.
- Transparency of savings calculation.
- Trust and privacy in Live Orders.
- Checkout friction after accepting an offer.
- Accessibility of dynamic product cards, price sliders, and payment choices.
- Hinglish content clarity and consistency.
