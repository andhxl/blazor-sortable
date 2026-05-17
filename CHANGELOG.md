# Changelog

## 8.0.2

### Fixes

- Reverted touch-specific multi-drag key behavior added in `8.0.1` because it caused multi-drag selection regressions after moving items between lists.

## 8.0.1

### Changed

- Renamed the unmatched attributes parameter from `Attributes` to `AdditionalAttributes`.
  This affects only consumers who set the capture-unmatched-values parameter explicitly by name.

### Fixes

- Fixed multi-drag selection on touch devices by ignoring `MultiDragKey` for touch input.

## 8.0.0

### Breaking Changes

- Renamed `Sortable` parameters:
  - `Pull` -> `PullMode`
  - `Put` -> `PutMode`
  - `KeySelector` -> `ItemKeySelector`
  - `DraggableSelector` -> `DraggableItemSelector`
- Changed `ConvertFunction` from `Func<SortableTransferContext<object>, TItem?>?` to `SortableTryConvertFunc<TItem>?`.
  The new delegate follows the `Try*` pattern and returns `false` when conversion fails.
- Changed `OnUpdate`, `OnAdd`, and `OnRemove` from `Action<SortableEventArgs<TItem>>?` to `EventCallback<SortableEventArgs<TItem>>`.
- Changed `SortableEventArgs<TItem>.From` and `SortableEventArgs<TItem>.To` from `ISortableInfo` to `SortableInfo`.
- Changed `SortableTransferContext<TItem>.From` and `SortableTransferContext<TItem>.To` from `ISortableInfo` to `SortableInfo`.
- Extended `SortableEventArgs<TItem>` with operation metadata, multi-drag item/index collections, and swap information.
  Code that directly calls the positional record constructor must be updated.
- Extended `SortableTransferContext<TItem>` with the `Items` collection.
  Code that directly calls the positional record constructor must be updated.
- Changed per-component behavior overrides to nullable values that fall back to `SortableOptions.Defaults`:
  `Delay`, `DelayOnTouchOnly`, `TouchStartThreshold`, `Animation`, `ForceFallback`,
  `FallbackOnBody`, `FallbackTolerance`, and `Scroll`.
- Changed effective defaults when component parameters are not set:
  `Delay` from `0` to `150`, `DelayOnTouchOnly` from `false` to `true`,
  `TouchStartThreshold` from `0` to `4`, and `FallbackTolerance` from `0` to `3`.
- Removed the public `ISortableInfo` contract from event/context payloads.
  Use `SortableInfo` instead.

### Added

- Added automatic loading for the bundled SortableJS script.
- Added automatic loading for the bundled BlazorSortable stylesheet.
- Added `SortableOptions` and `SortableDefaults` for configuring asset loading and default Sortable behavior.
- Added the bundled SortableJS 1.15.7 script to the package.
- Added `UseItemKeys` for disabling Blazor item keys when duplicate item keys can appear in the same list.
- Added multi-drag support:
  `MultiDrag`, `SelectedClass`, `MultiDragKey`, `AvoidImplicitDeselect`, `OnSelect`,
  `OnDeselect`, and `SortableSelectionEventArgs<TItem>`.
- Added swap support:
  `Swap`, `SwapClass`, `IsSwap`, and swap-aware add/remove/update handling.
- Added spill support:
  `RevertOnSpill`, `EmptyInsertThreshold`, `OnSpill`, and `SortableSpillEventArgs<TItem>`.
- Added `OnChange`, raised after update, add, and remove operations.
- Added `SortableChangeOperation` to identify the kind of list change in event args.
- Added `Items`, `OldIndexes`, and `NewIndexes` metadata for multi-drag operations.
- Added default helper styles for multi-drag selection and swap highlighting.

### Changed

- Lowered the library target framework from `net8.0` to `net6.0`.

### Fixes

- Reverted DOM moves consistently for add, remove, and update operations before updating the .NET state.
- Moved component callbacks to the `EventCallback` pipeline while keeping the component render after callback completion.

### Migration

#### Renamed parameters

Update renamed `Sortable` parameters:

Before:

```razor
<Sortable Items="items"
          Pull="SortablePullMode.Clone"
          Put="SortablePutMode.True"
          KeySelector="x => x.Id"
          DraggableSelector="x => x.CanDrag" />
```

After:

```razor
<Sortable Items="items"
          PullMode="SortablePullMode.Clone"
          PutMode="SortablePutMode.True"
          ItemKeySelector="x => x.Id"
          DraggableItemSelector="x => x.CanDrag" />
```

#### Event callbacks

`OnUpdate`, `OnAdd`, and `OnRemove` are now Blazor `EventCallback<SortableEventArgs<TItem>>` parameters
instead of `Action<SortableEventArgs<TItem>>?`.

If a handler previously called `StateHasChanged()` only to refresh the component after the callback,
that call can usually be removed because `EventCallback` participates in Blazor's normal render pipeline.

In some cases Razor may no longer infer `TItem` from the callback alone. If type inference fails,
specify the item type explicitly:

```razor
<Sortable TItem="Item"
          Items="items"
          OnUpdate="OnUpdate" />
```

#### SortableInfo

`From` and `To` now expose `SortableInfo`, which is a snapshot of sortable identity information and does not expose the component instance.

If your code only used `Id` or `Group`, no behavioral change is required.

#### ConvertFunction

Update conversion logic to the new `Try*` delegate shape.

Before:

```csharp
private Item? Convert(SortableTransferContext<object> context)
{
    return context.Item is OtherItem other
        ? new Item(other.Value)
        : null;
}
```

After:

```csharp
private bool TryConvert(SortableTransferContext<object> context, out Item item)
{
    if (context.Item is OtherItem other)
    {
        item = new Item(other.Value);
        return true;
    }

    item = default!;
    return false;
}
```

#### Component defaults

Component parameters such as `Delay`, `Animation`, `ForceFallback`, `FallbackTolerance`, and `Scroll`
now use `SortableOptions.Defaults` when they are not set on the component.

The following effective defaults changed from the previous version:

| Option | Previous default | 8.0.0 default |
|--------|------------------|---------------|
| `Delay` | `0` | `150` |
| `DelayOnTouchOnly` | `false` | `true` |
| `TouchStartThreshold` | `0` | `4` |
| `FallbackTolerance` | `0` | `3` |

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
