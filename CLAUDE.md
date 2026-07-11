# Budget

## C# conventions

- Do not suffix method names with `Async`, even for async methods (e.g. `Get()`, not `GetAsync()`). This applies to methods we write; third-party library APIs (e.g. `MongoDB.Driver`) keep their own naming.
- Do not default to `*OrDefault` methods (`FirstOrDefault`, `SingleOrDefault`, etc.) out of habit. Prefer throwing variants (`First`, `Single`) unless a missing result is a genuinely expected, legitimate case for that query.

## Blazor conventions

- Money amounts in the UI must always show exactly 2 digits after the decimal point (e.g. `29.98`, `50.00`, not `50`). Raw stored amount strings aren't guaranteed to have 2 decimals, so parse as `decimal` and format with `.ToString("0.00", CultureInfo.InvariantCulture)` rather than displaying the raw string.
- For any `@rendermode InteractiveAuto` page/component that fetches data in `OnInitializedAsync`, persist the fetched data via `[PersistentState]` properties and guard the fetch (e.g. `if (SomeProp is not null) { return; }`). Otherwise the component's `OnInitializedAsync` reruns and refetches everything a second time when the page hands off from interactive Server rendering to WebAssembly, causing a visible content "reload" flash. See `Transactions.razor`'s `TransactionList` and `Merchants.razor`'s `MerchantNames`/`Mappings` for the established pattern.
