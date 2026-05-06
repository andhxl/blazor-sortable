# Changelog

## 8.0.0

### Breaking Changes

- Changed `OnUpdate`, `OnAdd`, and `OnRemove` from `Action<SortableEventArgs<TItem>>?` to `EventCallback<SortableEventArgs<TItem>>`.
- Changed `SortableEventArgs<TItem>.From` and `SortableEventArgs<TItem>.To` from `ISortableInfo` to `SortableInfo`.
- Changed `SortableTransferContext<TItem>.From` and `SortableTransferContext<TItem>.To` from `ISortableInfo` to `SortableInfo`.

### Added

- Added automatic loading for the bundled SortableJS script.
- Added automatic loading for the bundled BlazorSortable stylesheet.
- Added `SortableOptions` for configuring BlazorSortable asset loading.
- Added the bundled SortableJS 1.15.7 script to the package.

### Changed

- Renamed the package JavaScript module from `blazor-sortable.js` to `Sortable.razor.js`.
- Updated package metadata and included license and third-party notices in the package.

### Fixes

- Reverted DOM moves consistently for add, remove, and update operations before updating the .NET state.
- Moved component callbacks to the `EventCallback` pipeline while keeping the component render after callback completion.

### Migration

#### Event callbacks

Update handlers from `Action<SortableEventArgs<TItem>>` assignments to normal Blazor event callback usage.

Before:

```razor
<Sortable Items="@items"
          OnUpdate="@OnUpdate" />
```

```csharp
private void OnUpdate(SortableEventArgs<Item> args)
{
    // ...
}
```

After:

```razor
<Sortable Items="@items"
          OnUpdate="OnUpdate" />
```

```csharp
private Task OnUpdate(SortableEventArgs<Item> args)
{
    // ...
    return Task.CompletedTask;
}
```

`void` handlers are still supported by Blazor, but `Task` is recommended for asynchronous work.

#### SortableInfo

`From` and `To` now expose `SortableInfo`, which is a snapshot of sortable identity information and does not expose the component instance.

If your code only used `Id` or `Group`, no behavioral change is required.

#### Asset loading

Manual SortableJS and stylesheet references are no longer required by default.

To keep managing assets yourself, disable automatic loading:

```csharp
builder.Services.AddSortable(options =>
{
    options.AutoLoadSortableJs = false;
    options.AutoLoadStylesheet = false;
});
```

When `AutoLoadSortableJs` is disabled, SortableJS must be available globally as `Sortable` before any `Sortable` component initializes.

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
