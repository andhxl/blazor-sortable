# BlazorSortable

![BlazorSortable Icon](https://raw.githubusercontent.com/andhxl/blazor-sortable/main/icon.png)

[![NuGet Version](https://img.shields.io/nuget/vpre/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)

A Blazor component that wraps [SortableJS](https://github.com/SortableJS/Sortable), a library for drag-and-drop sorting.

Inspired by [BlazorSortable](https://github.com/the-urlist/BlazorSortable) and represents an improved and extended implementation.

## Installation

### Via NuGet Package Manager

### Via .NET CLI

```bash
dotnet add package BlazorSortable
```

### Via PackageReference

Add to your .csproj file:
```xml
<ItemGroup>
  <PackageReference Include="BlazorSortable" Version="7.*" />
</ItemGroup>
```

## Setup

1. Add the SortableJS script to the host page of your app, before the closing `</body>` tag:
    - `wwwroot/index.html` (for Blazor WebAssembly Standalone App)
    - `Components/App.razor` (for Blazor Web App)

    Use one of the following methods:

    a) **Via CDN:**

    ```html
    <script src="https://cdn.jsdelivr.net/npm/sortablejs@1.15.7/Sortable.min.js"></script>
    ```

    b) **Locally:**

    Download SortableJS 1.15.7 ([GitHub](https://github.com/SortableJS/Sortable/releases/tag/1.15.7)), create `wwwroot/lib/sortablejs/`, and place `Sortable.min.js` in that folder.

    ```html
    <script src="lib/sortablejs/Sortable.min.js"></script>
    ```

    > Caching behavior depends on your hosting setup and static file cache headers.
    >
    > For **.NET 9+ Blazor Web App**, you can use static asset fingerprinting from a Razor file:
    > ```razor
    > <script src="@Assets["lib/sortablejs/Sortable.min.js"]"></script>
    > ```
    >
    > For **.NET 10+ Blazor WebAssembly Standalone Apps**, configure the app `.csproj` and use a fingerprint placeholder:
    > ```xml
    > <PropertyGroup>
    >   <OverrideHtmlAssetPlaceholders>true</OverrideHtmlAssetPlaceholders>
    > </PropertyGroup>
    > <ItemGroup>
    >   <StaticWebAssetFingerprintPattern Include="JS" Pattern="*.js" Expression="#[.{fingerprint}]!" />
    > </ItemGroup>
    > ```
    > ```html
    > <script src="lib/sortablejs/Sortable.min#[.{fingerprint}].js"></script>
    > ```
    >
    > If needed, you can add a query string that matches the SortableJS version:
    > ```html
    > <script src="lib/sortablejs/Sortable.min.js?v=1.15.7"></script>
    > ```

2. (Optional) Add base styles to the `<head>` element of the same host page:
    ```html
    <link rel="stylesheet" href="_content/BlazorSortable/blazor-sortable.css" />
    ```

    > For **.NET 9+ Blazor Web App**, you can use static asset fingerprinting from a Razor file:
    > ```razor
    > <link rel="stylesheet" href="@Assets["_content/BlazorSortable/blazor-sortable.css"]" />
    > ```
    >
    > From a Razor file, you can also use the current BlazorSortable assembly version as a cache-busting query string:
    > ```razor
    > <link rel="stylesheet" href="_content/BlazorSortable/blazor-sortable.css?v=@(typeof(BlazorSortable.Sortable<>).Assembly.GetName().Version)" />
    > ```
    >
    >> For **Blazor WebAssembly Standalone App**, use the assembly-version query string inside the `<HeadContent>` section of `App.razor` and make sure `Program.cs` contains:
    >> ```csharp
    >> builder.RootComponents.Add<HeadOutlet>("head::after");
    >> ```

3. Add services in the `Program.cs` of the app where the component runs (`InteractiveWebAssembly` components require registration in the client app):
    ```csharp
    using BlazorSortable;

    // ...

    builder.Services.AddSortable();
    ```

4. Add the using directive in `_Imports.razor`:
    ```razor
    @using BlazorSortable
    ```

## Usage Example

```razor
<div class="sortable-sample">
    <Sortable Items="@(["Write docs", "Add tests", "Publish package"])"
              Group="tasks"
              Class="sortable-column">
        <div class="sortable-item">
            @context
        </div>
    </Sortable>

    <Sortable Items="@(["Create project"])"
              Group="tasks"
              Class="sortable-column">
        <div class="sortable-item">
            @context
        </div>
    </Sortable>

    <Sortable TItem="object"
              Group="delete"
              Put="SortablePutMode.Groups"
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
        cursor: grab;
        user-select: none;
    }
</style>
```

## Component Parameters

### Sortable

> **Note:** Support for MultiDrag and Swap plugins will be added in future releases.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `TItem` | `notnull` | - | Type of items displayed, sorted, or accepted by the component |
| `Items` | `IList<TItem>?` | `null` | Items to display and sort. If `null`, the component works as a drop zone |
| `ChildContent` | `RenderFragment<TItem>?` | `null` | Template for displaying each item. Can be a component, HTML elements, or any Razor markup. Ignored when `Items` is `null` |
| `Context` | `string` | `context` | Razor template context name used by `ChildContent` to refer to the current item |
| `KeySelector` | `Func<TItem, object>?` | `null` | Function for generating the key used in `@key`. If not set, the item itself is used |
| `Class` | `string?` | `null` | CSS class for the container |
| `Style` | `string?` | `null` | Inline CSS styles for the container |
| `Attributes` | `IReadOnlyDictionary<string, object>?` | `null` | Additional custom attributes that will be rendered by the component |
| `Id` | `string` | `Random GUID` | Unique identifier of the component. Used internally for coordination between Sortable components. Must be globally unique |
| `Group` | `string` | `Random GUID` | Name of the group for interacting with other Sortable components |
| `Pull` | `SortablePullMode?` | `null` | Mode for pulling items from this Sortable component |
| `PullGroups` | `string[]?` | `null` | **Required when `Pull="SortablePullMode.Groups"`.** Specifies the target groups into which items from this Sortable component can be dragged |
| `CloneFunction` | `Func<TItem, TItem>?` | `null` | **Required when `Pull="SortablePullMode.Clone"`.** A factory method used to create a non-null clone of the dragged item |
| `PullFunction` | `Predicate<SortableTransferContext<TItem>>?` | `null` | **Required when `Pull="SortablePullMode.Function"`.** Function to determine whether an item can be pulled to the target Sortable component. **Works only when the component runs on WebAssembly.** |
| `Put` | `SortablePutMode?` | `null` | Mode for accepting items into this Sortable component |
| `PutGroups` | `string[]?` | `null` | **Required when `Put="SortablePutMode.Groups"`.** Specifies the source groups from which this Sortable component can accept items |
| `PutFunction` | `Predicate<SortableTransferContext<object>>?` | `null` | **Required when `Put="SortablePutMode.Function"`.** Function to determine whether an item can be accepted by this Sortable component. **Works only when the component runs on WebAssembly.** |
| `ConvertFunction` | `Func<SortableTransferContext<object>, TItem?>?` | `null` | Converts incoming items that are not assignable to the target item type. Return `null` when conversion is not possible |
| `Sort` | `bool` | `true` | Enables or disables sorting within this Sortable component |
| `Delay` | `int` | `0` | Time in milliseconds to define when sorting should start. Unfortunately, due to browser restrictions, delaying is not possible on IE or Edge with native drag and drop |
| `DelayOnTouchOnly` | `bool` | `false` | Whether the delay should be applied only when the user is using touch, e.g. on a mobile device. No delay will be applied in any other case |
| `TouchStartThreshold` | `int` | `0` | Minimum pointer movement that must occur before delayed sorting is cancelled. Values between `3` and `5` are good |
| `Disabled` | `bool` | `false` | Disables the Sortable component when set to true |
| `Animation` | `int` | `150` | Animation duration in milliseconds |
| `Handle` | `string?` | `null` | CSS selector for elements that can be dragged, e.g. `.my-handle` |
| `Filter` | `string?` | `null` | CSS selector for elements that cannot be dragged, e.g. `.ignore-elements` |
| `DraggableSelector` | `Predicate<TItem>?` | `null` | Function to determine whether an item can be dragged |
| `DraggableClass` | `string` | `"sortable-draggable"` | CSS class applied to items that can be dragged |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the placeholder during drag |
| `ChosenClass` | `string` | `"sortable-chosen"` | CSS class for the chosen element |
| `DragClass` | `string` | `"sortable-drag"` | CSS class for the dragged element |
| `SwapThreshold` | `double` | `1` | Percentage of the target that the swap zone will take up, as a float between `0` and `1` |
| `InvertSwap` | `bool` | `false` | Set to true to set the swap zone to the sides of the target, for the effect of sorting "in between" items |
| `InvertedSwapThreshold` | `double` | `1` | Percentage of the target that the inverted swap zone will take up, as a float between `0` and `1` |
| `ForceFallback` | `bool` | `true` | If set to true, the fallback for non-HTML5 browsers will be used, even if an HTML5 browser is used |
| `FallbackClass` | `string` | `"sortable-fallback"` | CSS class for the element in fallback mode |
| `FallbackOnBody` | `bool` | `false` | Appends the cloned DOM element to the document body |
| `FallbackTolerance` | `int` | `0` | Emulates the native drag threshold. Specify in pixels how far the mouse should move before it is considered a drag. Values between `3` and `5` are good |
| `Scroll` | `bool` | `true` | Enables scrolling of the container during dragging |
| `OnUpdate` | `Action<SortableEventArgs<TItem>>?` | `null` | Raised when the order of items is changed |
| `OnAdd` | `Action<SortableEventArgs<TItem>>?` | `null` | Raised when an item is accepted by the component |
| `OnRemove` | `Action<SortableEventArgs<TItem>>?` | `null` | Raised when an item is removed from the component |

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

All events receive a `SortableEventArgs<TItem>` parameter.  
Functions like `PullFunction`, `PutFunction` and `ConvertFunction` use a `SortableTransferContext<T>` parameter.

### SortableEventArgs

The `SortableEventArgs<TItem>` class provides information about sorting operations.

| Property | Type | Description |
|----------|------|-------------|
| `Item` | `TItem` | The item participating in the operation |
| `From` | `ISortableInfo` | Source sortable component |
| `OldIndex` | `int` | The previous index of the item in the source sortable |
| `To` | `ISortableInfo` | Target sortable component |
| `NewIndex` | `int` | The new index of the item in the target sortable |
| `IsClone` | `bool` | Flag indicating whether the item is a clone |

### SortableTransferContext

The `SortableTransferContext<TItem>` class represents the context of transferring an item between sortable components.

| Property | Type | Description |
|----------|------|-------------|
| `Item` | `TItem` | The item being transferred between sortable components |
| `From` | `ISortableInfo` | The source sortable component |
| `To` | `ISortableInfo` | The target sortable component |

### ISortableInfo

The `ISortableInfo` interface provides information about a sortable component.

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | Unique identifier of the component |
| `Group` | `string` | Group name for interaction with other Sortable components |

## Notes

- **Order of events when dragging between components:**
    1. `OnAdd` is triggered **first** - during this event, the item is **still present in the source component**.
    2. `OnRemove` is triggered **after**.

- **Events use `Action<T>?` instead of `EventCallback<T>`:**

    `EventCallback<T>` uses the component event pipeline, which calls `StateHasChanged()` on the receiving `ComponentBase` after the handler runs. This causes conflicts between the DOM and the data model for this component.

- **Type mismatch / failed conversion:**

    If the dragged item is assignable to the target item type, it is added as-is and `ConvertFunction` is not called.  
    If the item is not assignable, `ConvertFunction` is used when provided.  
    If no `ConvertFunction` is provided, or it returns `null`, the item is not accepted and remains in its original position.

- **Dragging issues on scrolled page:**

    If the dragged element appears misaligned when the page is scrolled, set
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
