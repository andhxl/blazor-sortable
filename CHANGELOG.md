# Changelog

## 7.0.0

### Breaking Changes

- Changed `ConvertFunction` behavior. It is now called only when the incoming item is not assignable to the target item type.
- Moved package static files from subfolders to the package static asset root.

### Fixes

- Moved the component's internal `StateHasChanged()` call to run after `OnUpdate`, `OnAdd`, and `OnRemove`, so the component no longer renders an intermediate state before user handlers finish.

### Migration

#### ConvertFunction

If you used `ConvertFunction` for same-type preprocessing or filtering, move preprocessing to `OnAdd` and filtering to `PutFunction` where supported, or to application-specific wrapper logic.

#### Static Files

Update the stylesheet path by removing the `css` segment:

```html
<link rel="stylesheet" href="_content/BlazorSortable/blazor-sortable.css" />
```
