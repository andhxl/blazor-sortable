# BlazorSortable

![BlazorSortable Icon](https://raw.githubusercontent.com/andhxl/blazor-sortable/main/icon.png)

[![NuGet Version](https://img.shields.io/nuget/vpre/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)

A Blazor component that wraps the [SortableJS](https://github.com/SortableJS/Sortable) library, designed for creating interactive sortable lists with drag-and-drop support. Inspired by [BlazorSortable](https://github.com/the-urlist/BlazorSortable) and represents an improved and extended implementation.

## Installation

### Via Nuget Package Manager

### Via .NET CLI
```bash
dotnet add package BlazorSortable
```

### Via PackageReference
Add to your .csproj file:
```xml
<ItemGroup>
  <PackageReference Include="BlazorSortable" Version="5.*" />
</ItemGroup>
```

## Setup

1. Add the SortableJS library to:
    - `wwwroot/index.html` (for Blazor WebAssembly)  
    - `Components/App.razor` (for Blazor Web App)  
    - `Pages/_Host.cshtml` (for Blazor Server)

    Using one of the following methods:

    a) **Via CDN:**
    ```html
    <script src="https://cdn.jsdelivr.net/npm/sortablejs@latest/Sortable.min.js"></script>
    ```

    b) **Locally:**
    ```html
    <script src="lib/sortable/dist/js/Sortable.min.js"></script>
    ```
    > You can optionally include a version parameter to avoid browser caching:
    > ```html
    > <script src="lib/sortable/dist/js/Sortable.min.js?v=1.15.6"></script>
    > ```

    For local installation:
    1. Download the latest version of SortableJS from [GitHub](https://github.com/SortableJS/Sortable/releases)
    2. Create the folder structure in `wwwroot`: `lib/sortable/dist/js/`
    3. Place the `Sortable.min.js` file in the created folder

2. (Optional) Add base styles to the same file where you added the script:
```html
<link rel="stylesheet" href="_content/BlazorSortable/css/blazor-sortable.css" />
```
> You can also specify the version manually to prevent browser caching:
> ```html
> <link rel="stylesheet" href="_content/BlazorSortable/css/blazor-sortable.css?v=5.1.6" />
> ```
> Or automatically insert the current assembly version (works in `.razor` or `.cshtml` files).
> Add this code within the `<head>` element, or for **Blazor WebAssembly**, place it inside the `<HeadContent>` section of `App.razor`:
> ```razor
> <link rel="stylesheet" href="_content/BlazorSortable/css/blazor-sortable.css?v=@(typeof(BlazorSortable.Sortable<>).Assembly.GetName().Version)" />
> ```
> > For this to work in **Blazor WebAssembly**, make sure you have the following line in your `Program.cs`:
> > ```csharp
> > builder.RootComponents.Add<HeadOutlet>("head::after");
> > ```

3. Add services in `Program.cs`:
```csharp
using BlazorSortable;

// ...

builder.Services.AddSortable();
```

4. Add the using directive in `_Imports.razor`:
```razor
@using BlazorSortable
```

## Usage Examples

```razor
<Sortable Items="Persons"
          Class="my-sortable"
          Group="group1">
    <PersonComponent Person="context" />
</Sortable>
```

```razor
<Sortable TItem="Person"
          Items="Persons"
          Class="my-sortable"
          Group="group1"
          Context="person">
    <div class="person-card">
        <h4>@person.Name</h4>
        <p>@person.Email</p>
        <span class="badge">@person.Department</span>
    </div>
</Sortable>
```

```razor
<Sortable TItem="object"
          Class="my-sortable-drop-zone"
          Group="group1" />
```

## Component Parameters

### Sortable

> **Note:** Support for MultiDrag and Swap plugins will be added in future releases.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `TItem` | — | — | The type of items in the list |
| `Items` | `IList<TItem>?` | `null` | List of items to display and sort |
| `ChildContent` | `RenderFragment<TItem>?` | `null` | Template for displaying each list item. Can be a component, HTML elements, or any Razor markup |
| `KeySelector` | `Func<TItem, object>?` | `null` | Function for generating the key used in `@key`. If not set, the item itself is used |
| `Context` | `string` | `context` | Name of the parameter used in the child content template to refer to the current item |
| `Class` | `string?` | `null` | CSS class for the container |
| `Style` | `string?` | `null` | Inline CSS styles for the container |
| `Attributes` | `IReadOnlyDictionary<string, object>?` | `null` | Additional custom attributes that will be rendered by the component |
| `Id` | `string` | `Random GUID` | Unique identifier of the component. Used internally for coordination between Sortable components. Must be globally unique |
| `Group` | `string` | `Random GUID` | Name of the group for interacting with other sortable instances |
| `Pull` | `SortablePullMode?` | `null` | Mode for pulling items from the list |
| `PullGroups` | `string[]?` | `null` | **Required when `Pull="SortablePullMode.Groups"`.** Specifies the groups into which items from this list can be dragged |
| `CloneFunction` | `Func<TItem, TItem>?` | `null` | **Required when `Pull="SortablePullMode.Clone"`.** A factory method used to create a clone of the dragged item |
| `PullFunction` | `Predicate<SortableTransferContext<TItem>>?` | `null` | **Required when `Pull="SortablePullMode.Function"`.** Function to determine if an item can be pulled to the target Sortable component. **Works only in Blazor WebAssembly** |
| `Put` | `SortablePutMode?` | `null` | Mode for adding items to the list |
| `PutGroups` | `string[]?` | `null` | **Required when `Put="SortablePutMode.Groups"`.** Specifies the groups from which items are allowed to be added |
| `PutFunction` | `Predicate<SortableTransferContext<object>>?` | `null` | **Required when `Put="SortablePutMode.Function"`.** Function to determine if an item can be put into this list. **Works only in Blazor WebAssembly** |
| `ConvertFunction` | `Func<SortableTransferContext<object>, TItem?>?` | `null` | Function to convert items from other Sortable component to the target type |
| `Sort` | `bool` | `true` | Enables or disables sorting of items within the list |
| `Delay` | `int` | `0` | Time in milliseconds to define when the sorting should start. Unfortunately, due to browser restrictions, delaying is not possible on IE or Edge with native drag and drop |
| `DelayOnTouchOnly` | `bool` | `false` | Whether or not the delay should be applied only if the user is using touch (eg. on a mobile device). No delay will be applied in any other case |
| `TouchStartThreshold` | `int` | `0` | This option sets the minimum pointer movement that must occur before the delayed sorting is cancelled. Values between `3` to `5` are good |
| `Disabled` | `bool` | `false` | Disables the Sortable component when set to true |
| `Animation` | `int` | `150` | Animation duration in milliseconds |
| `Handle` | `string?` | `null` | CSS selector for elements that can be dragged (e.g. `.my-handle`) |
| `Filter` | `string?` | `null` | CSS selector for elements that cannot be dragged (e.g. `.ignore-elements`) |
| `DraggableSelector` | `Predicate<TItem>?` | `null` | Function to determine if an item can be dragged |
| `DraggableClass` | `string` | `"sortable-draggable"` | CSS class applied to items that can be dragged |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the placeholder during drag |
| `ChosenClass` | `string` | `"sortable-chosen"` | CSS class for the chosen element |
| `DragClass` | `string` | `"sortable-drag"` | CSS class for the dragged element |
| `SwapThreshold` | `double` | `1` | Percentage of the target that the swap zone will take up, as a float between `0` and `1` |
| `InvertSwap` | `bool` | `false` | Set to true to set the swap zone to the sides of the target, for the effect of sorting "in between" items |
| `InvertedSwapThreshold` | `double` | `1` | Percentage of the target that the inverted swap zone will take up, as a float between `0` and `1` |
| `ForceFallback` | `bool` | `true` | If set to true, the Fallback for non HTML5 Browser will be used, even if we are using an HTML5 Browser |
| `FallbackClass` | `string` | `"sortable-fallback"` | CSS class for the element in fallback mode |
| `FallbackOnBody` | `bool` | `false` | Appends the cloned DOM Element into the Document's Body |
| `FallbackTolerance` | `int` | `0` | Emulates the native drag threshold. Specify in pixels how far the mouse should move before it's considered as a drag. `3` to `5` are probably good values |
| `Scroll` | `bool` | `true` | Enables scrolling of the container during dragging |
| `OnUpdate` | `Action<SortableEventArgs<TItem>>?` | `null` | Raised when the order of items is changed |
| `OnAdd` | `Action<SortableEventArgs<TItem>>?` | `null` | Raised when an item is added to the list |
| `OnRemove` | `Action<SortableEventArgs<TItem>>?` | `null` | Raised when an item is removed from the list |

### SortablePullMode

| Value | Description |
|-------|-------------|
| `True` | Allows pulling items from the list |
| `False` | Prohibits pulling items from the list |
| `Groups` | Allows pulling items only from specified groups (requires `PullGroups` parameter) |
| `Clone` | Creates a clone of the item when dragging (requires `CloneFunction` parameter) |
| `Function` | Uses a custom function to determine if items can be pulled (requires `PullFunction` parameter) |

### SortablePutMode

| Value | Description |
|-------|-------------|
| `True` | Allows adding items to the list |
| `False` | Prohibits adding items to the list |
| `Groups` | Allows adding items only from specified groups (requires `PutGroups` parameter) |
| `Function` | Uses a custom function to determine if items can be added (requires `PutFunction` parameter) |

## Events

All events receive a `SortableEventArgs<TItem>` parameter.  
Functions like `PullFunction`, `PutFunction` and `ConvertFunction` use a `SortableTransferContext<TItem>` parameter.

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

- **Order of events when dragging between lists:**
  1. `OnAdd` is triggered **first** — during this event, the item is **still present in the source list**.
  2. `OnRemove` is triggered **after**.

- **Events use `Action<T>?` instead of `EventCallback<T>`.**  
  **Reason:** `EventCallback.InvokeAsync` automatically triggers `ComponentBase.StateHasChanged` in the parent component, which causes conflicts between the DOM and the data model for this component.

- **Type mismatch / failed conversion:**  
  If item types don’t match or the `ConvertFunction` returns `null`, the item is **not added** to the target list and is **remains in its original position**.

- **Dragging issues on scrolled page:**  
  If the dragged element appears misaligned when the page is scrolled, set
  ```razor
  ForceFallback="false"
  ```
  **or**
  ```razor
  FallbackOnBody="true"
  ```

- **Blazor Server limitation:**  
  `PullFunction` and `PutFunction` require synchronous JS-to-.NET calls used by Sortable.js,
  which are only available in **Blazor WebAssembly**.
  On Blazor Server these functions cannot work and will throw `NotSupportedException`.
