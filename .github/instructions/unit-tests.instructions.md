---
applyTo: "src/**/*Tests.cs"
---

# Unit test conventions

- Name tests with the `When..._And..._Then...` pattern: the trigger comes first, each extra condition is prefixed with `And`, and the expected result follows `Then`. Example: `WhenTemplateIdIsMissing_AndUkprnIsPositive_ThenValidationFailsOnTemplateIdOnly`.
- Use NUnit `Assert.Multiple` to group related independent assertions. Keep null/validity guards outside the group so later assertions cannot dereference a failed value.
- Prefer one test file per method under test (for example `UserSettingsServiceTests\GetUserSettingsTests.cs`), reusing shared setup/helpers where appropriate.
- Use AutoFixture data attributes (`AutoData`, `InlineAutoData`, `MoqAutoData`, `MoqInlineAutoData`) and set only the properties relevant to each test rather than relying on class-level test data.
- Use `[Frozen]` for mocks that are shared with the system under test, and `[Greedy]` when the system under test is an MVC controller.
- Use `RecursiveMoqAutoData` when an entity instance is needed.
