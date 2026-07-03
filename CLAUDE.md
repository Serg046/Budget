# Budget

## C# conventions

- Do not suffix method names with `Async`, even for async methods (e.g. `Get()`, not `GetAsync()`). This applies to methods we write; third-party library APIs (e.g. `MongoDB.Driver`) keep their own naming.
- Do not default to `*OrDefault` methods (`FirstOrDefault`, `SingleOrDefault`, etc.) out of habit. Prefer throwing variants (`First`, `Single`) unless a missing result is a genuinely expected, legitimate case for that query.
