using System.ComponentModel;
using BlazorSortable.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BlazorSortable;

/// <summary>
/// Component for sorting and transferring items with drag and drop.
/// </summary>
/// <typeparam name="TItem">Type of items displayed, sorted, or accepted by the component.</typeparam>
public sealed partial class Sortable<TItem> : ISortableList, IAsyncDisposable
    where TItem : notnull
{
    /// <summary>
    /// Items to display and sort. If null, the component works as a drop zone.
    /// </summary>
    [Parameter]
    public IList<TItem>? Items { get; set; }

    /// <summary>
    /// Template for displaying each item. Can be a component, HTML elements, or any Razor markup.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Items"/> is not null.
    /// </remarks>
    [Parameter]
    public RenderFragment<TItem>? ChildContent { get; set; }

    /// <summary>
    /// Uses Blazor keys for rendered item wrappers.
    /// </summary>
    /// <remarks>
    /// Enabled by default to preserve item identity and child component state during reordering.
    /// Disable only when the same item key can appear more than once in the same Sortable component.
    /// </remarks>
    [Parameter]
    public bool UseItemKeys { get; set; } = true;

    /// <summary>
    /// Function used to generate a stable Blazor key for each rendered item.
    /// </summary>
    /// <remarks>
    /// Keys must be unique among items rendered by the same Sortable component.
    /// If not provided, the item itself is used as the key.
    /// Used only when <see cref="UseItemKeys"/> is true.
    /// </remarks>
    [Parameter]
    public Func<TItem, object>? ItemKeySelector { get; set; }

    /// <summary>
    /// CSS class applied to the root container of the Sortable component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Inline CSS styles applied to the root container of the Sortable component.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Specifies additional custom attributes that will be rendered by the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object?>? Attributes { get; set; }

    /// <summary>
    /// Unique identifier of the component. Must be globally unique across all Sortable instances.
    /// </summary>
    /// <remarks>
    /// If not set explicitly, a GUID will be generated automatically.
    /// This ID is required for internal coordination between Sortable components.
    /// Set this manually only if you need to identify the component externally.
    /// </remarks>
    [Parameter]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Group name for interaction with other Sortable components.
    /// </summary>
    [Parameter]
    public string Group { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Mode for pulling items from this Sortable component.
    /// </summary>
    [Parameter]
    public SortablePullMode? PullMode { get; set; }

    /// <summary>
    /// Array of target group names into which items from this Sortable component can be dragged.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="PullMode"/> is set to <see cref="SortablePullMode.Groups"/>.
    /// </remarks>
    [Parameter]
    public string[]? PullGroups { get; set; }

    /// <summary>
    /// Factory method used to create a non-null clone of the dragged item.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="PullMode"/> is set to <see cref="SortablePullMode.Clone"/>.
    /// </remarks>
    [Parameter]
    public Func<TItem, TItem>? CloneFunction { get; set; }

    /// <summary>
    /// Function used to determine whether an item can be pulled to the target Sortable component.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="PullMode"/> is set to <see cref="SortablePullMode.Function"/>.
    /// This feature works only when the component runs on WebAssembly.
    /// SortableJS requires a synchronous JS-to-.NET call, which is not supported
    /// outside of WebAssembly, for example with server-side interactivity.
    /// </remarks>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when used outside of WebAssembly.
    /// </exception>
    [Parameter]
    public Predicate<SortableTransferContext<TItem>>? PullFunction { get; set; }

    /// <summary>
    /// Mode for accepting items into this Sortable component.
    /// </summary>
    [Parameter]
    public SortablePutMode? PutMode { get; set; }

    /// <summary>
    /// Array of source group names from which this Sortable component can accept items.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="PutMode"/> is set to <see cref="SortablePutMode.Groups"/>.
    /// </remarks>
    [Parameter]
    public string[]? PutGroups { get; set; }

    /// <summary>
    /// Function used to determine whether an item can be accepted by this Sortable component.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="PutMode"/> is set to <see cref="SortablePutMode.Function"/>.
    /// This feature works only when the component runs on WebAssembly.
    /// SortableJS requires a synchronous JS-to-.NET call, which is not supported
    /// outside of WebAssembly, for example with server-side interactivity.
    /// </remarks>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown when used outside of WebAssembly.
    /// </exception>
    [Parameter]
    public Predicate<SortableTransferContext<object>>? PutFunction { get; set; }

    /// <summary>
    /// Function used to convert incoming items that are not assignable to <typeparamref name="TItem"/>.
    /// </summary>
    /// <remarks>
    /// Used when an item is transferred from another Sortable component and cannot be cast to
    /// <typeparamref name="TItem"/> directly. Return <see langword="false"/> when conversion is not possible.
    /// </remarks>
    [Parameter]
    public SortableTryConvertFunc<TItem>? ConvertFunction { get; set; }

    /// <summary>
    /// Enables or disables sorting within this Sortable component.
    /// </summary>
    [Parameter]
    public bool Sort { get; set; } = true;

    /// <summary>
    /// Time in milliseconds before sorting starts.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.Delay"/> is used.
    /// </remarks>
    [Parameter]
    public int? Delay { get; set; }

    /// <summary>
    /// Whether the delay should be applied only for touch input.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.DelayOnTouchOnly"/> is used.
    /// </remarks>
    [Parameter]
    public bool? DelayOnTouchOnly { get; set; }

    /// <summary>
    /// When the <see cref="Delay"/> option is set, some phones with very sensitive touch displays like the Samsung Galaxy S8 will fire unwanted touchmove events even when your finger is not moving, resulting in the sort not triggering.
    /// This option sets the minimum pointer movement that must occur before the delayed sorting is cancelled.
    /// Values between 3 to 5 are good.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.TouchStartThreshold"/> is used.
    /// </remarks>
    [Parameter]
    public int? TouchStartThreshold { get; set; }

    /// <summary>
    /// Disables the Sortable component when set to true.
    /// When disabled, drag and drop operations are not allowed.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Animation duration in milliseconds. 0 - without animation.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.Animation"/> is used.
    /// </remarks>
    [Parameter]
    public int? Animation { get; set; }

    /// <summary>
    /// CSS selector for elements that can be used for dragging.
    /// Example: ".my-handle" - dragging only by elements with class my-handle
    /// </summary>
    [Parameter]
    public string? Handle { get; set; }

    /// <summary>
    /// CSS selector for elements that cannot be dragged.
    /// Example: ".ignore-elements" - dragging disabled for elements with class ignore-elements
    /// </summary>
    [Parameter]
    public string? Filter { get; set; }

    /// <summary>
    /// Function used to determine if an item can be dragged.
    /// </summary>
    /// <remarks>
    /// If provided, only items that return true from this function will be draggable.
    /// The draggable class will be applied to items that return true.
    /// </remarks>
    [Parameter]
    public Predicate<TItem>? DraggableItemSelector { get; set; }

    /// <summary>
    /// CSS class applied to items that can be dragged.
    /// </summary>
    /// <remarks>
    /// Used in conjunction with <see cref="DraggableItemSelector"/> to style draggable items.
    /// </remarks>
    [Parameter]
    public string DraggableClass { get; set; } = "sortable-draggable";

    /// <summary>
    /// CSS class for the ghost element during dragging.
    /// </summary>
    [Parameter]
    public string GhostClass { get; set; } = "sortable-ghost";

    /// <summary>
    /// CSS class for the chosen element.
    /// </summary>
    [Parameter]
    public string ChosenClass { get; set; } = "sortable-chosen";

    /// <summary>
    /// CSS class for the dragged element.
    /// </summary>
    [Parameter]
    public string DragClass { get; set; } = "sortable-drag";

    /// <summary>
    /// Percentage of the target that the swap zone will take up, as a float between 0 and 1.
    /// </summary>
    [Parameter]
    public double SwapThreshold { get; set; } = 1;

    /// <summary>
    /// Set to true to set the swap zone to the sides of the target, for the effect of sorting "in between" items.
    /// </summary>
    [Parameter]
    public bool InvertSwap { get; set; }

    /// <summary>
    /// Percentage of the target that the inverted swap zone will take up, as a float between 0 and 1.
    /// </summary>
    [Parameter]
    public double InvertedSwapThreshold { get; set; } = 1;

    /// <summary>
    /// Forces SortableJS to use fallback drag behavior instead of native HTML5 drag and drop.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.ForceFallback"/> is used.
    /// The fallback generates a copy of the DOM element and applies <see cref="FallbackClass"/>.
    /// </remarks>
    [Parameter]
    public bool? ForceFallback { get; set; }

    /// <summary>
    /// CSS class for the element in fallback mode.
    /// </summary>
    [Parameter]
    public string FallbackClass { get; set; } = "sortable-fallback";

    /// <summary>
    /// Appends the cloned DOM element to the document body.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.FallbackOnBody"/> is used.
    /// </remarks>
    [Parameter]
    public bool? FallbackOnBody { get; set; }

    /// <summary>
    /// Minimum pointer movement, in pixels, before fallback dragging starts.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.FallbackTolerance"/> is used.
    /// Values between 3 and 5 are usually good for clickable items.
    /// </remarks>
    [Parameter]
    public int? FallbackTolerance { get; set; }

    /// <summary>
    /// Distance, in pixels, from an empty sortable container at which an item can be inserted.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.EmptyInsertThreshold"/> is used.
    /// </remarks>
    [Parameter]
    public int? EmptyInsertThreshold { get; set; }

    /// <summary>
    /// Enables multi-drag functionality.
    /// </summary>
    [Parameter]
    public bool MultiDrag { get; set; }

    /// <summary>
    /// CSS class for selected items in multi-drag mode.
    /// </summary>
    [Parameter]
    public string SelectedClass { get; set; } = "sortable-selected";

    /// <summary>
    /// Key used to enable multi-drag selection.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.MultiDragKey"/> is used.
    /// Users must hold this key while clicking to select multiple items.
    /// Set to an empty string to allow multi-drag selection without holding a modifier key.
    /// </remarks>
    [Parameter]
    public string? MultiDragKey { get; set; }

    /// <summary>
    /// Prevents automatic deselection when clicking on selected items.
    /// </summary>
    /// <remarks>
    /// When true, clicking on a selected item will not deselect it.
    /// Useful for maintaining selection state during complex interactions.
    /// </remarks>
    [Parameter]
    public bool AvoidImplicitDeselect { get; set; }

    /// <summary>
    /// Enables swap mode for dragging.
    /// </summary>
    /// <remarks>
    /// When enabled, dragging an item over another item will swap their positions
    /// instead of inserting the dragged item at the new position.
    /// </remarks>
    [Parameter]
    public bool Swap { get; set; }

    /// <summary>
    /// CSS class applied to items during swap highlighting.
    /// </summary>
    /// <remarks>
    /// Applied to items that would be swapped when <see cref="Swap"/> is enabled.
    /// </remarks>
    [Parameter]
    public string SwapClass { get; set; } = "sortable-swap-highlight";

    /// <summary>
    /// Enables scrolling of the container during dragging.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.Scroll"/> is used.
    /// </remarks>
    [Parameter]
    public bool? Scroll { get; set; }

    /// <summary>
    /// Reverts the dragged item when it is dropped outside a valid sortable target.
    /// </summary>
    /// <remarks>
    /// If not set, <see cref="SortableDefaults.RevertOnSpill"/> is used.
    /// </remarks>
    [Parameter]
    public bool? RevertOnSpill { get; set; }

    /// <summary>
    /// Callback invoked after the order of items is changed.
    /// </summary>
    [Parameter]
    public EventCallback<SortableEventArgs<TItem>> OnUpdate { get; set; }

    /// <summary>
    /// Callback invoked after an item is accepted by the component.
    /// </summary>
    [Parameter]
    public EventCallback<SortableEventArgs<TItem>> OnAdd { get; set; }

    /// <summary>
    /// Callback invoked after an item is removed from the component.
    /// </summary>
    [Parameter]
    public EventCallback<SortableEventArgs<TItem>> OnRemove { get; set; }

    /// <summary>
    /// Callback invoked after an update, add, or remove operation.
    /// </summary>
    [Parameter]
    public EventCallback<SortableEventArgs<TItem>> OnChange { get; set; }

    /// <summary>
    /// Event that occurs when an item is selected in multi-drag mode.
    /// </summary>
    [Parameter]
    public EventCallback<SortableSelectionEventArgs<TItem>> OnSelect { get; set; }

    /// <summary>
    /// Event that occurs when an item is deselected in multi-drag mode.
    /// </summary>
    [Parameter]
    public EventCallback<SortableSelectionEventArgs<TItem>> OnDeselect { get; set; }

    /// <summary>
    /// Event that occurs when an item is dropped outside a valid sortable target.
    /// </summary>
    [Parameter]
    public EventCallback<SortableSpillEventArgs<TItem>> OnSpill { get; set; }

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
    [Inject] private SortableRegistry Registry { get; set; } = default!;
    [Inject] private IOptions<SortableOptions> Options { get; set; } = default!;

    private IJSObjectReference? jsModule;
    private DotNetObjectReference<Sortable<TItem>>? selfReference;

    private int draggedItemIndex = -1;
    private int[] draggedItemIndexes = [];

    private bool shouldSkipNextRemove;

    private TItem pendingSwapItem = default!;
    private bool hasPendingSwapItem;

    private object GetItemKey(TItem item) =>
        ItemKeySelector?.Invoke(item) ?? item;

    private string? GetItemClass(TItem item) =>
        DraggableItemSelector?.Invoke(item) == true ? DraggableClass : null;

    /// <inheritdoc/>
    protected override void OnParametersSet()
    {
        switch (PullMode)
        {
            case SortablePullMode.Groups:
                ArgumentNullException.ThrowIfNull(PullGroups);
                break;
            case SortablePullMode.Clone:
                ArgumentNullException.ThrowIfNull(CloneFunction);
                break;
            case SortablePullMode.Function:
                ArgumentNullException.ThrowIfNull(PullFunction);
                break;
        }

        switch (PutMode)
        {
            case SortablePutMode.Groups:
                ArgumentNullException.ThrowIfNull(PutGroups);
                break;
            case SortablePutMode.Function:
                ArgumentNullException.ThrowIfNull(PutFunction);
                break;
        }

        if (Swap && MultiDrag)
        {
            throw new InvalidOperationException(
                $"{nameof(Swap)} cannot be used together with {nameof(MultiDrag)}.");
        }
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        // Check WebAssembly-only options here because OnParametersSet can run during
        // server prerendering, where InteractiveWebAssembly components are not in the browser yet.
        if ((PullMode == SortablePullMode.Function || PutMode == SortablePutMode.Function) &&
            !OperatingSystem.IsBrowser())
        {
            throw new PlatformNotSupportedException(
                $"{nameof(PullFunction)} and {nameof(PutFunction)} are only supported when the component runs on WebAssembly.");
        }

        jsModule = await JsRuntime.InvokeAsync<IJSObjectReference>("import",
            $"./_content/BlazorSortable/Sortable.razor.js?v={AssemblyVersion.Value}");

        selfReference = DotNetObjectReference.Create(this);

        await jsModule.InvokeVoidAsync("initSortable",
            Id,
            BuildSortableOptions(),
            selfReference,
            new
            {
                autoLoadSortableJs = Options.Value.AutoLoadSortableJs,
                autoLoadStylesheet = Options.Value.AutoLoadStylesheet,
                version = AssemblyVersion.Value
            });

        Registry.Register(Id, this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (jsModule is not null)
        {
            try
            {
                await jsModule.InvokeVoidAsync("destroySortable", Id);
                await jsModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Ignore disconnected server-side circuits
            }
        }

        // Dispose selfReference after jsModule
        selfReference?.Dispose();

        Registry.Unregister(Id);
    }

    private Dictionary<string, object> BuildSortableOptions()
    {
        var group = new Dictionary<string, object>
        {
            ["name"] = Group
        };

        var pull = GetPull();
        if (pull is not null)
            group["pull"] = pull;

        var put = GetPut();
        if (put is not null)
            group["put"] = put;

        var defaults = Options.Value.Defaults;

        var options = new Dictionary<string, object>
        {
            ["group"] = group,
            ["sort"] = Sort,
            ["delay"] = Delay ?? defaults.Delay,
            ["delayOnTouchOnly"] = DelayOnTouchOnly ?? defaults.DelayOnTouchOnly,
            ["touchStartThreshold"] = TouchStartThreshold ?? defaults.TouchStartThreshold,
            ["disabled"] = Disabled,
            ["animation"] = Animation ?? defaults.Animation,
            ["ghostClass"] = GhostClass,
            ["chosenClass"] = ChosenClass,
            ["dragClass"] = DragClass,
            ["swapThreshold"] = SwapThreshold,
            ["invertSwap"] = InvertSwap,
            ["invertedSwapThreshold"] = InvertedSwapThreshold,
            ["forceFallback"] = ForceFallback ?? defaults.ForceFallback,
            ["fallbackClass"] = FallbackClass,
            ["fallbackOnBody"] = FallbackOnBody ?? defaults.FallbackOnBody,
            ["fallbackTolerance"] = FallbackTolerance ?? defaults.FallbackTolerance,
            ["emptyInsertThreshold"] = EmptyInsertThreshold ?? defaults.EmptyInsertThreshold,
        };

        if (!string.IsNullOrWhiteSpace(Handle))
            options["handle"] = Handle;

        if (!string.IsNullOrWhiteSpace(Filter))
            options["filter"] = Filter;

        if (DraggableItemSelector is not null)
            options["draggable"] = "." + DraggableClass;

        if (MultiDrag)
        {
            options["multiDrag"] = true;
            options["selectedClass"] = SelectedClass;
            options["avoidImplicitDeselect"] = AvoidImplicitDeselect;
            options["multiDragKey"] = MultiDragKey ?? defaults.MultiDragKey ?? string.Empty;
        }

        if (Swap)
        {
            options["swap"] = true;
            options["swapClass"] = SwapClass;
        }

        options["scroll"] = Scroll ?? defaults.Scroll;

        options["revertOnSpill"] = RevertOnSpill ?? defaults.RevertOnSpill;
        // removeOnSpill mutates the DOM without raising onRemove and can still be followed
        // by MultiDrag update events, so the wrapper only exposes revertOnSpill.
        options["removeOnSpill"] = false;

        return options;
    }

    private object? GetPull() =>
        PullMode switch
        {
            SortablePullMode.True => true,
            SortablePullMode.False => false,
            SortablePullMode.Groups => PullGroups,
            SortablePullMode.Clone => "clone",
            SortablePullMode.Function => "function",
            _ => null
        };

    private object? GetPut() =>
        PutMode switch
        {
            SortablePutMode.True => true,
            SortablePutMode.False => false,
            SortablePutMode.Groups => PutGroups,
            SortablePutMode.Function => "function",
            _ => null
        };

#pragma warning disable CS1591, IDE1006

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public void OnStartJs(int index, int[] indexes)
    {
        draggedItemIndex = index;
        draggedItemIndexes = indexes;
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public void OnEndJs()
    {
        draggedItemIndex = -1;
        draggedItemIndexes = [];
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public bool OnPullJs(string toId)
    {
        var to = Registry[toId];

        var item = Items![draggedItemIndex];
        var items = draggedItemIndexes.Select(i => Items[i]).ToArray();

        return PullFunction!(new(
            item, items, CreateInfo(this), CreateInfo(to)));
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public bool OnPutJs(string fromId)
    {
        var from = Registry[fromId];

        var item = from.GetTransferItem(from.DraggedItemIndex);
        var items = from.DraggedItemIndexes.Select(from.GetTransferItem).ToArray();

        return PutFunction!(new(
            item, items, CreateInfo(from), CreateInfo(this)));
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public async Task OnUpdateJs(
        int oldIndex,
        int[] oldIndexes,
        int newIndex,
        int[] newIndexes,
        bool isSwap)
    {
        var item = Items![oldIndex];
        var items = oldIndexes.Select(i => Items[i]).ToArray();

        var isMultiDragOperation = oldIndexes.Length > 0;

        if (isMultiDragOperation)
        {
            var moves = oldIndexes
                .Select((index, i) => new
                {
                    OldIndex = index,
                    NewIndex = newIndexes[i],
                    Item = Items[index]
                })
                .ToArray();

            // SortableJS MultiDrag reports old indexes in ascending order.
            // Remove from the end so earlier removals do not shift later indexes.
            for (var i = moves.Length - 1; i >= 0; i--)
                Items.RemoveAt(moves[i].OldIndex);

            // SortableJS MultiDrag reports new indexes in ascending order.
            // Insert in that order so each item lands at its reported final index.
            foreach (var move in moves)
                Items.Insert(move.NewIndex, move.Item);
        }
        else if (isSwap)
        {
            (Items[oldIndex], Items[newIndex]) = (Items[newIndex], Items[oldIndex]);
        }
        else
        {
            Items.RemoveAt(oldIndex);
            Items.Insert(newIndex, item);
        }

        try
        {
            if (OnUpdate.HasDelegate || OnChange.HasDelegate)
            {
                var info = CreateInfo(this);

                var args = new SortableEventArgs<TItem>(
                    SortableChangeOperation.Update,
                    item, items, info, oldIndex, oldIndexes, info, newIndex, newIndexes, false, isSwap);

                if (OnUpdate.HasDelegate)
                    await OnUpdate.InvokeAsync(args);

                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync(args);
            }
        }
        finally
        {
            StateHasChanged();
        }
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public async Task OnAddJs(
        string fromId,
        int oldIndex,
        int[] oldIndexes,
        int newIndex,
        int[] newIndexes,
        bool isClone,
        bool isSwap)
    {
        var from = Registry[fromId];
        from.ShouldSkipNextRemove = true;

        var isMultiDragOperation = oldIndexes.Length > 0;

        var sourceIndexes = isMultiDragOperation
            ? oldIndexes
            : [oldIndex];

        var sourceObjects = sourceIndexes
            .Select(from.GetTransferItem)
            .ToArray();

        var fromInfo = CreateInfo(from);
        var toInfo = CreateInfo(this);

        var convertedByOldIndex = new Dictionary<int, TItem>();

        for (var i = 0; i < sourceObjects.Length; i++)
        {
            if (!TryConvertTransferItem(sourceObjects[i], sourceObjects, fromInfo, toInfo, out var convertedItem))
                return;

            convertedByOldIndex[sourceIndexes[i]] = convertedItem;
        }

        if (Items is not null)
        {
            if (isMultiDragOperation)
            {
                for (var i = 0; i < oldIndexes.Length; i++)
                    Items.Insert(newIndexes[i], convertedByOldIndex[oldIndexes[i]]);
            }
            else if (isSwap)
            {
                // Swap sends the target item back to the source, so apply this list's pull behavior.
                var swapItem = GetTransferItem(newIndex);

                // The swap item is transferred in the opposite direction: from this target list back to the source list.
                if (!from.TrySetPendingSwapItem(swapItem, toInfo, fromInfo))
                    return;

                Items[newIndex] = convertedByOldIndex[oldIndex];
            }
            else
            {
                Items.Insert(newIndex, convertedByOldIndex[oldIndex]);
            }
        }

        from.ShouldSkipNextRemove = false;

        try
        {
            if (OnAdd.HasDelegate || OnChange.HasDelegate)
            {
                var item = convertedByOldIndex[oldIndex];
                var items = oldIndexes.Select(i => convertedByOldIndex[i]).ToArray();

                var args = new SortableEventArgs<TItem>(
                    SortableChangeOperation.Add,
                    item, items, fromInfo, oldIndex, oldIndexes, toInfo, newIndex, newIndexes, isClone, isSwap);

                if (OnAdd.HasDelegate)
                    await OnAdd.InvokeAsync(args);

                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync(args);
            }
        }
        finally
        {
            StateHasChanged();
        }
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public async Task OnRemoveJs(
        int oldIndex,
        int[] oldIndexes,
        string toId,
        int newIndex,
        int[] newIndexes,
        bool isClone)
    {
        if (shouldSkipNextRemove)
        {
            shouldSkipNextRemove = false;
            return;
        }

        var isSwap = hasPendingSwapItem;

        if (isClone && !isSwap)
            return;

        // Capture the removed items before mutating the collection
        var item = Items![oldIndex];
        var items = oldIndexes.Select(i => Items[i]).ToArray();

        var isMultiDragOperation = oldIndexes.Length > 0;

        if (isSwap)
        {
            if (isClone)
            {
                Items.Insert(oldIndex, pendingSwapItem);
            }
            else
            {
                Items[oldIndex] = pendingSwapItem;
            }

            pendingSwapItem = default!;
            hasPendingSwapItem = false;
        }
        else if (isMultiDragOperation)
        {
            for (var i = oldIndexes.Length - 1; i >= 0; i--)
                Items.RemoveAt(oldIndexes[i]);
        }
        else
        {
            Items.RemoveAt(oldIndex);
        }

        try
        {
            if (OnRemove.HasDelegate || OnChange.HasDelegate)
            {
                var fromInfo = CreateInfo(this);
                var toInfo = CreateInfo(Registry[toId]);

                var args = new SortableEventArgs<TItem>(
                    SortableChangeOperation.Remove,
                    item, items, fromInfo, oldIndex, oldIndexes, toInfo, newIndex, newIndexes, isClone, isSwap);

                if (OnRemove.HasDelegate)
                    await OnRemove.InvokeAsync(args);

                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync(args);
            }
        }
        finally
        {
            StateHasChanged();
        }
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public async Task OnSelectJs(int index, int[] selectedIndexes)
    {
        if (!OnSelect.HasDelegate)
            return;

        var item = Items![index];
        var selectedItems = selectedIndexes.Select(i => Items[i]).ToArray();

        await OnSelect.InvokeAsync(new(item, selectedItems));
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public async Task OnDeselectJs(int index, int[] selectedIndexes)
    {
        if (!OnDeselect.HasDelegate)
            return;

        var item = Items![index];
        var selectedItems = selectedIndexes.Select(i => Items[i]).ToArray();

        await OnDeselect.InvokeAsync(new(item, selectedItems));
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public async Task OnSpillJs(int index, int[] selectedIndexes, bool isClone)
    {
        if (!OnSpill.HasDelegate)
            return;

        var item = Items![index];
        var selectedItems = selectedIndexes.Select(i => Items[i]).ToArray();

        await OnSpill.InvokeAsync(new(item, selectedItems, isClone));
    }

#pragma warning restore CS1591, IDE1006

    private bool TryConvertTransferItem(
        object sourceObject,
        IReadOnlyList<object> sourceObjects,
        SortableInfo from,
        SortableInfo to,
        out TItem item)
    {
        if (sourceObject is TItem sourceItem)
        {
            item = sourceItem;
            return true;
        }

        if (ConvertFunction is null)
        {
            item = default!;
            return false;
        }

        return ConvertFunction(
            new(sourceObject, sourceObjects, from, to),
            out item);
    }

    private object GetTransferItem(int index)
    {
        var item = Items![index];

        return PullMode == SortablePullMode.Clone
            ? CloneFunction!(item)
            : item;
    }

    object ISortableList.GetTransferItem(int index) => GetTransferItem(index);

    bool ISortableList.TrySetPendingSwapItem(object item, SortableInfo from, SortableInfo to)
    {
        if (!TryConvertTransferItem(item, [], from, to, out var convertedItem))
            return false;

        pendingSwapItem = convertedItem;
        hasPendingSwapItem = true;

        return true;
    }

    int ISortableList.DraggedItemIndex => draggedItemIndex;

    IReadOnlyList<int> ISortableList.DraggedItemIndexes => draggedItemIndexes;

    bool ISortableList.ShouldSkipNextRemove
    {
        get => shouldSkipNextRemove;
        set => shouldSkipNextRemove = value;
    }

    private static SortableInfo CreateInfo(ISortableInfo sortable) =>
        new(sortable.Id, sortable.Group);
}
