# Changelog

## 7.0.0

### Breaking Changes

- Changed `ConvertFunction` behavior. It is now called only when the incoming item is not assignable to the target item type.
- Moved package static files from subfolders to the package static asset root.

### Fixes

- Moved the component's internal `StateHasChanged()` call after `OnUpdate`, `OnAdd`, and `OnRemove`, so item changes made inside handlers are reflected immediately after the drag operation.

### Migration From 6.x

#### ConvertFunction

If you used `ConvertFunction` for same-type preprocessing or filtering, move preprocessing to `OnAdd` and filtering to `PutFunction` where supported, or to application-specific wrapper logic.

#### Static Files

Update the stylesheet path by removing the `css` segment:

```html
<link rel="stylesheet" href="_content/BlazorSortable/blazor-sortable.css" />
```
