# BlazorSortable

![BlazorSortable Icon](https://raw.githubusercontent.com/andhxl/blazor-sortable/main/icon.png)

[![NuGet Version](https://img.shields.io/nuget/vpre/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)

A Blazor component that wraps [SortableJS](https://github.com/SortableJS/Sortable), a JavaScript library for reorderable drag-and-drop lists.

Inspired by [BlazorSortable](https://github.com/the-urlist/BlazorSortable) and represents an improved and extended implementation.

## Installation

### Via NuGet Package Manager

Search for BlazorSortable in NuGet Package Manager.

### Via .NET CLI

```bash
dotnet add package BlazorSortable
```

### Via PackageReference

Add to your .csproj file:
```xml
<ItemGroup>
  <PackageReference Include="BlazorSortable" Version="8.*" />
</ItemGroup>
```

## Setup

1. Add BlazorSortable services in the `Program.cs` file of the app where the component runs (`InteractiveWebAssembly` components require registration in the client app):

    ```csharp
    using BlazorSortable;

    // ...

    builder.Services.AddSortable();
    ```

2. Add the using directive in `_Imports.razor`:

    ```razor
    @using BlazorSortable
    ```

By default, BlazorSortable loads the bundled SortableJS 1.15.7 script, the base stylesheet, and the highlight stylesheet automatically when the first `Sortable` component is initialized.

### SortableOptions

`AddSortable` accepts an optional configuration delegate:

```csharp
builder.Services.AddSortable(options =>
{
    options.AutoLoadStylesheet = false;
    options.Defaults.ForceFallback = false;
});
```

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `AutoLoadSortableJs` | `bool` | `true` | Loads the bundled SortableJS script automatically |
| `AutoLoadStylesheet` | `bool` | `true` | Loads the bundled BlazorSortable stylesheet automatically |
| `AutoLoadHighlightStylesheet` | `bool` | `true` | Loads the bundled BlazorSortable highlight stylesheet automatically when `AutoLoadStylesheet` is enabled |
| `Defaults` | `SortableDefaults` | `new()` | Default SortableJS behavior used when a component parameter is not set |

You can also pass a preconfigured `SortableOptions` instance:

```csharp
builder.Services.AddSortable(new SortableOptions
{
    AutoLoadStylesheet = false,
    Defaults =
    {
        ForceFallback = false,
        RevertOnSpill = true
    }
});
```

### SortableDefaults

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Delay` | `int` | `150` | Time in milliseconds before sorting starts |
| `DelayOnTouchOnly` | `bool` | `true` | Applies `Delay` only for touch input |
| `TouchStartThreshold` | `int` | `4` | Minimum touch movement, in pixels, before delayed sorting is cancelled |
| `Animation` | `int` | `150` | Animation duration in milliseconds |
| `ForceFallback` | `bool` | `true` | Forces SortableJS to use fallback drag behavior instead of native HTML5 drag and drop |
| `FallbackOnBody` | `bool` | `false` | Appends the fallback clone element to the document body |
| `FallbackTolerance` | `int` | `3` | Minimum pointer movement, in pixels, before fallback dragging starts |
| `EmptyInsertThreshold` | `int` | `5` | Distance, in pixels, from an empty sortable container at which an item can be inserted |
| `MultiDragKey` | `string` | `"Control"` | Key used to select multiple items in multi-drag mode for mouse input. Touch input does not require a modifier key. Set to an empty string to select without holding a modifier key for all input types |
| `Scroll` | `bool` | `true` | Enables automatic scrolling while dragging near scroll container edges |
| `RevertOnSpill` | `bool` | `false` | Reverts the dragged item when it is dropped outside a valid sortable target |

### Manual Asset Loading

If you want to manage these assets yourself, disable automatic loading:

```csharp
builder.Services.AddSortable(options =>
{
    options.AutoLoadSortableJs = false;
    options.AutoLoadStylesheet = false;
});
```

When `AutoLoadSortableJs` is disabled, SortableJS must be available globally as `Sortable` before any `Sortable` component initializes.

If you want to keep the base helper styles but provide your own selected item and swap highlight styles, disable only the highlight stylesheet:

```csharp
builder.Services.AddSortable(options =>
{
    options.AutoLoadHighlightStylesheet = false;
});
```

### Bundled Styles

The bundled base stylesheet `_content/BlazorSortable/blazor-sortable.css` contains only the default helper styles used by BlazorSortable:

```css
.sortable-ghost {
    visibility: hidden;
}

.sortable-fallback {
    opacity: 1 !important;
}
```

The bundled highlight stylesheet `_content/BlazorSortable/blazor-sortable-highlights.css` contains the default visual styles for multi-drag selection and swap highlighting:

```css
.sortable-selected {
    outline: var(--blazor-sortable-selected-outline-width, 2px) solid var(--blazor-sortable-selected-outline-color, Highlight);
    outline-offset: calc(-1 * var(--blazor-sortable-selected-outline-width, 2px));
}

.sortable-swap-highlight {
    outline: var(--blazor-sortable-swap-highlight-outline-width, 2px) solid var(--blazor-sortable-swap-highlight-outline-color, Highlight);
    outline-offset: calc(-1 * var(--blazor-sortable-swap-highlight-outline-width, 2px));
}
```

You can disable all automatic stylesheet loading with `AutoLoadStylesheet = false` if you want to provide these styles yourself.

## Usage Example

```razor
<div class="sortable-sample">
    <Sortable Items="todoItems"
              Group="tasks"
              Class="sortable-column">
        <div class="sortable-item">
            @context
        </div>
    </Sortable>

    <Sortable Items="doneItems"
              Group="tasks"
              Class="sortable-column">
        <div class="sortable-item">
            @context
        </div>
    </Sortable>

    <Sortable TItem="object"
              Group="delete"
              PutMode="SortablePutMode.Groups"
              PutGroups="@(["tasks"])"
              Class="sortable-delete" />
</div>

<style>
    .sortable-sample {
        display: flex;
        gap: 16px;
    }

    .sortable-column,
    .sortable-delete {
        width: 180px;
        min-height: 96px;
        padding: 12px;
        border: 2px dashed #d0d7de;
        border-radius: 6px;
    }

    .sortable-column {
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .sortable-delete {
        border-color: #dc3545;
        overflow: hidden;
    }

    .sortable-item {
        padding: 10px 12px;
        border: 1px solid #d0d7de;
        border-radius: 6px;
        background: white;
        color: #24292f;
        cursor: grab;
        user-select: none;
    }
</style>

@code {
    private readonly IList<string> todoItems = ["Write docs", "Add tests", "Publish package"];
    private readonly IList<string> doneItems = ["Create project"];
}
```

## API

### Sortable

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `TItem` | `notnull` | - | Type of items displayed, sorted, or accepted by the component |
| `Items` | `IList<TItem>?` | `null` | Optional backing list for items displayed, sorted, or accepted by the component. When `null`, accepted drops are reported through events but not stored locally |
| `ChildContent` | `RenderFragment<TItem>?` | `null` | Template for rendering each item from `Items`. If omitted, the component does not render items. When `Items` is provided, accepted drops are still stored in that list |
| `Context` | `string` | `context` | Razor template context name used by `ChildContent`. This is a Razor template attribute, not a component parameter |
| `UseItemKeys` | `bool` | `true` | Enables Blazor `@key` for rendered item wrappers |
| `ItemKeySelector` | `Func<TItem, object>?` | `null` | Function for generating a stable item key. If not set, the item itself is used |
| `Class` | `string?` | `null` | CSS class for the container |
| `Style` | `string?` | `null` | Inline CSS styles for the container |
| `AdditionalAttributes` | `IReadOnlyDictionary<string, object>?` | `null` | Additional custom attributes that will be rendered by the component |
| `Id` | `string` | `Random GUID` | Unique identifier of the component. Used internally for coordination between Sortable components. Must be globally unique |
| `Group` | `string` | `Random GUID` | Name of the group for interacting with other Sortable components |
| `PullMode` | `SortablePullMode?` | `null` | Mode for pulling items from this Sortable component |
| `PullGroups` | `string[]?` | `null` | **Required when `PullMode="SortablePullMode.Groups"`.** Specifies the target groups into which items from this Sortable component can be dragged |
| `CloneFunction` | `Func<TItem, TItem>?` | `null` | **Required when `PullMode="SortablePullMode.Clone"`.** A factory method used to create a non-null clone of the dragged item |
| `PullFunction` | `Predicate<SortableTransferContext<TItem>>?` | `null` | **Required when `PullMode="SortablePullMode.Function"`.** Function to determine whether an item can be pulled to the target Sortable component. **Works only when the component runs on WebAssembly.** |
| `PutMode` | `SortablePutMode?` | `null` | Mode for accepting items into this Sortable component |
| `PutGroups` | `string[]?` | `null` | **Required when `PutMode="SortablePutMode.Groups"`.** Specifies the source groups from which this Sortable component can accept items |
| `PutFunction` | `Predicate<SortableTransferContext<object>>?` | `null` | **Required when `PutMode="SortablePutMode.Function"`.** Function to determine whether an item can be accepted by this Sortable component. **Works only when the component runs on WebAssembly.** |
| `ConvertFunction` | `SortableTryConvertFunc<TItem>?` | `null` | Converts incoming items that are not assignable to the target item type. Used for cross-list transfers between Sortable components with different item types. Return `false` when conversion is not possible |
| `Sort` | `bool` | `true` | Enables or disables sorting within this Sortable component |
| `Delay` | `int?` | `Defaults.Delay` (`150`) | Time in milliseconds to define when sorting should start |
| `DelayOnTouchOnly` | `bool?` | `Defaults.DelayOnTouchOnly` (`true`) | Whether the delay should be applied only when the user is using touch |
| `TouchStartThreshold` | `int?` | `Defaults.TouchStartThreshold` (`4`) | Minimum pointer movement that must occur before delayed sorting is cancelled. Values between `3` and `5` are good |
| `Disabled` | `bool` | `false` | Disables the Sortable component when set to true |
| `Animation` | `int?` | `Defaults.Animation` (`150`) | Animation duration in milliseconds |
| `Handle` | `string?` | `null` | CSS selector for elements that can be dragged, e.g. `.my-handle` |
| `Filter` | `string?` | `null` | CSS selector for elements that cannot be dragged, e.g. `.ignore-elements` |
| `DraggableItemSelector` | `Predicate<TItem>?` | `null` | Function to determine whether an item can be dragged |
| `DraggableClass` | `string` | `"sortable-draggable"` | CSS class applied to items that can be dragged |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the placeholder during drag |
| `ChosenClass` | `string` | `"sortable-chosen"` | CSS class for the chosen element |
| `DragClass` | `string` | `"sortable-drag"` | CSS class for the dragged element |
| `SwapThreshold` | `double` | `1` | Percentage of the target that the swap zone will take up, as a float between `0` and `1` |
| `InvertSwap` | `bool` | `false` | Set to true to set the swap zone to the sides of the target, for the effect of sorting "in between" items |
| `InvertedSwapThreshold` | `double` | `1` | Percentage of the target that the inverted swap zone will take up, as a float between `0` and `1` |
| `ForceFallback` | `bool?` | `Defaults.ForceFallback` (`true`) | If set to true, the fallback for non-HTML5 browsers will be used, even if an HTML5 browser is used |
| `FallbackClass` | `string` | `"sortable-fallback"` | CSS class for the element in fallback mode |
| `FallbackOnBody` | `bool?` | `Defaults.FallbackOnBody` (`false`) | Appends the cloned DOM element to the document body |
| `FallbackTolerance` | `int?` | `Defaults.FallbackTolerance` (`3`) | Emulates the native drag threshold. Specify in pixels how far the mouse should move before it is considered a drag |
| `EmptyInsertThreshold` | `int?` | `Defaults.EmptyInsertThreshold` (`5`) | Distance, in pixels, from an empty sortable container at which an item can be inserted |
| `MultiDrag` | `bool` | `false` | Enables multi-drag selection and moving multiple items together |
| `SelectedClass` | `string` | `"sortable-selected"` | CSS class for selected items in multi-drag mode |
| `MultiDragKey` | `string?` | `Defaults.MultiDragKey` (`"Control"`) | Key used to select multiple items in multi-drag mode for mouse input. Touch input does not require a modifier key. Set to an empty string to select without holding a modifier key for all input types |
| `AvoidImplicitDeselect` | `bool` | `false` | Prevents automatic deselection when clicking selected items in multi-drag mode |
| `Swap` | `bool` | `false` | Enables swap mode. Cannot be used together with `MultiDrag` |
| `SwapClass` | `string` | `"sortable-swap-highlight"` | CSS class applied to items during swap highlighting |
| `Scroll` | `bool?` | `Defaults.Scroll` (`true`) | Enables scrolling of the container during dragging |
| `RevertOnSpill` | `bool?` | `Defaults.RevertOnSpill` (`false`) | Reverts the dragged item when it is dropped outside a valid sortable target |
| `OnUpdate` | `EventCallback<SortableEventArgs<TItem>>` | `default` | Raised when the order of items is changed |
| `OnAdd` | `EventCallback<SortableEventArgs<TItem>>` | `default` | Raised when an item is accepted by the component |
| `OnRemove` | `EventCallback<SortableEventArgs<TItem>>` | `default` | Raised when an item is removed from the component |
| `OnChange` | `EventCallback<SortableEventArgs<TItem>>` | `default` | Raised after update, add, or remove operations |
| `OnSelect` | `EventCallback<SortableSelectionEventArgs<TItem>>` | `default` | Raised when an item is selected in multi-drag mode |
| `OnDeselect` | `EventCallback<SortableSelectionEventArgs<TItem>>` | `default` | Raised when an item is deselected in multi-drag mode |
| `OnSpill` | `EventCallback<SortableSpillEventArgs<TItem>>` | `default` | Raised when an item is dropped outside a valid sortable target |

### SortablePullMode

| Value | Description |
|-------|-------------|
| `True` | Allows pulling items |
| `False` | Prohibits pulling items |
| `Groups` | Allows pulling items only into specified target groups (requires `PullGroups` parameter) |
| `Clone` | Creates a clone of the item when dragging (requires `CloneFunction` parameter) |
| `Function` | Uses a custom function to determine whether items can be pulled (requires `PullFunction` parameter) |

### SortablePutMode

| Value | Description |
|-------|-------------|
| `True` | Allows accepting items |
| `False` | Prohibits accepting items |
| `Groups` | Allows accepting items only from specified source groups (requires `PutGroups` parameter) |
| `Function` | Uses a custom function to determine whether items can be accepted (requires `PutFunction` parameter) |

## Events

`OnUpdate`, `OnAdd`, `OnRemove`, and `OnChange` receive a `SortableEventArgs<TItem>` parameter.

`OnSelect` and `OnDeselect` receive a `SortableSelectionEventArgs<TItem>` parameter.

`OnSpill` receives a `SortableSpillEventArgs<TItem>` parameter.

Functions like `PullFunction`, `PutFunction`, and `ConvertFunction` use a `SortableTransferContext<T>` parameter.

### SortableEventArgs

The `SortableEventArgs<TItem>` class provides information about sorting operations.

| Property | Type | Description |
|----------|------|-------------|
| `Operation` | `SortableChangeOperation` | The operation that changed the list |
| `Item` | `TItem` | The primary item participating in the operation |
| `Items` | `IReadOnlyList<TItem>` | Items participating in a multi-drag operation. Empty for single-item operations |
| `From` | `SortableInfo` | Source sortable information |
| `OldIndex` | `int` | The previous index of the primary item in the source sortable |
| `OldIndexes` | `IReadOnlyList<int>` | Previous indexes for a multi-drag operation. Empty for single-item operations |
| `To` | `SortableInfo` | Target sortable information |
| `NewIndex` | `int` | The new index of the primary item in the target sortable |
| `NewIndexes` | `IReadOnlyList<int>` | New indexes for a multi-drag operation. Empty for single-item operations |
| `IsClone` | `bool` | Indicates whether the operation uses a cloned dragged item |
| `IsSwap` | `bool` | Indicates whether the dragged item was swapped with another item. The target swap item is not exposed separately |

### SortableChangeOperation

The `SortableChangeOperation` enum defines the operation that changed a Sortable component's items.

| Value | Description |
|-------|-------------|
| `Update` | Items were reordered within the same Sortable component |
| `Add` | Items were accepted by a Sortable component |
| `Remove` | Items were removed from a Sortable component |

### SortableSelectionEventArgs

The `SortableSelectionEventArgs<TItem>` class provides information about multi-drag selection changes.

| Property | Type | Description |
|----------|------|-------------|
| `Item` | `TItem` | The item whose selection state changed |
| `SelectedItems` | `IReadOnlyList<TItem>` | Selected items after the selection change |

### SortableSpillEventArgs

The `SortableSpillEventArgs<TItem>` class provides information about a spill operation.

| Property | Type | Description |
|----------|------|-------------|
| `Item` | `TItem` | The primary item dropped outside a valid sortable target |
| `Items` | `IReadOnlyList<TItem>` | Selected items for a multi-drag spill operation. Empty for single-item operations |
| `IsClone` | `bool` | Indicates whether the spill operation uses a cloned dragged item |

### SortableTransferContext

The `SortableTransferContext<TItem>` class represents the context of transferring an item between sortable components.

| Property | Type | Description |
|----------|------|-------------|
| `Item` | `TItem` | The primary item being transferred |
| `Items` | `IReadOnlyList<TItem>` | Items associated with the transfer when a collection is available |
| `From` | `SortableInfo` | The source sortable information |
| `To` | `SortableInfo` | The target sortable information |

### SortableInfo

The `SortableInfo` type provides sortable identity information.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | Unique identifier of the Sortable component |
| `Group` | `string` | Group name for interaction with other Sortable components |

### SortableTryConvertFunc<TItem>

The `SortableTryConvertFunc<TItem>` delegate represents a method that attempts to convert an incoming item from a sortable transfer context to a strongly typed item.

This delegate is used by the `ConvertFunction` parameter when items are transferred between `Sortable` components with different item types.

```csharp
public delegate bool SortableTryConvertFunc<TItem>(
    SortableTransferContext<object> context,
    out TItem item)
    where TItem : notnull;
```

The method should return `true` when conversion succeeds. If conversion is not possible, it should return `false` and set `item` to its default value.

| Parameter | Type | Description |
|-----------|------|-------------|
| `context` | `SortableTransferContext<object>` | Transfer context that contains the incoming item and source/target sortable information |
| `item` | `out TItem` | Converted item when conversion succeeds; otherwise the default value |

| Return value | Description |
|--------------|-------------|
| `true` | The item was converted successfully and can be accepted by the target Sortable component |
| `false` | The item could not be converted and should not be accepted |

## Notes

- **Order of events when moving items between components:**

    For a non-clone move between components:
    1. `OnAdd` is triggered **first** - during this event, the item is **still present in the source component**.
    2. `OnRemove` is triggered **after**.

- **Type mismatch / failed conversion:**

    If the dragged item is assignable to the target item type, it is added as-is and `ConvertFunction` is not called.  
    If the item is not assignable, `ConvertFunction` is used when provided.  
    If no `ConvertFunction` is provided, or it returns `false`, the item is not accepted and remains in its original position.
    `ConvertFunction` receives a `SortableTransferContext<object>` because the source item type may be different from the target `TItem`.

- **Moving items between lists with fallback mode:**

    Components that exchange items should use the same effective `ForceFallback` value. Mixing fallback and native HTML5 drag behavior between source and target lists can produce inconsistent DOM and pointer behavior during cross-list moves.

- **Do not create item lists inline in markup:**

    Pass a stable list instance from `@code`. Avoid expressions that create a new list on every render, for example:

    ```razor
    <Sortable Items="@(["One", "Two", "Three"])">
        @context
    </Sortable>
    ```

    Use a field or property with a stable backing collection instead:

    ```razor
    <Sortable Items="items">
        @context
    </Sortable>

    @code {
        private readonly IList<string> items = ["One", "Two", "Three"];
    }
    ```

- **Dragging issues on scrolled page:**

    If the dragged element appears misaligned when the page is scrolled inside a custom scroll container, set
    ```razor
    ForceFallback="false"
    ```
    or
    ```razor
    FallbackOnBody="true"
    ```

- **Server-side interactivity limitation:**

    `PullFunction` and `PutFunction` require synchronous JS-to-.NET calls used by SortableJS,
    which are only available when the component runs on WebAssembly.
    These options are not supported with server-side interactivity and will throw a `PlatformNotSupportedException`.
    > Note: Prerendering is supported, but these options require WebAssembly at runtime.
