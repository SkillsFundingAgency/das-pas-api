# User preferences

- Use NUnit `Assert.Multiple` to group related independent assertions, but keep null/validity guards outside the group so later assertions cannot dereference a failed value.
- Name tests with the `When..._Then...` pattern.
- Prefer one test file per repository method, reusing shared setup/helpers where appropriate.
- Use AutoFixture data in tests; set only the properties relevant to each test rather than relying on class-level test data.
- For methods returning collections, ensure that return value is never null
