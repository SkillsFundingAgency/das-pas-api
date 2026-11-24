# User preferences

- Use NUnit `Assert.Multiple` to group related independent assertions, but keep null/validity guards outside the group so later assertions cannot dereference a failed value.
- Name tests with the `When..._And..._Then...` pattern, where each condition is separated by `_And_` and the expected result is after `_Then_`.
- Prefer one test file per repository method, reusing shared setup/helpers where appropriate.	- Use AutoFixture's AutoData or InlineAutoData or MoqAutoData or InlineMoqAutoData in tests; set only the properties relevant to each test rather than relying on class-level test data.
	- use RecursiveMoqAutoData when an entity instance is needed
- For methods returning collections, ensure that return value is never null
