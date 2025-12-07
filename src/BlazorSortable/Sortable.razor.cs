using BlazorSortable.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace BlazorSortable;

// TODO: MultiDrag, Swap

/// <summary>
/// Component for creating sortable with drag and drop functionality.
/// </summary>
/// <typeparam name="TItem">Type of items in the list.</typeparam>
public partial class Sortable<TItem> : ISortableList, IAsyncDisposable
{
    /// <summary>
    /// List of items to display and sort.
    /// </summary>
    [Parameter]
    public IList<TItem>? Items { get; set; }

    /// <summary>
    /// Template for displaying each list item.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? ChildContent { get; set; }

    /// <summary>
    /// Function used to generate a stable key for each item, used in the <c>@key</c> directive for rendering.
    /// If not provided, the item itself is used as the key.
    /// </summary>
    [Parameter]
    public Func<TItem, object>? KeySelector { get; set; }

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
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

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
    /// Mode for pulling items from the list.
    /// </summary>
    [Parameter]
    public SortablePullMode? Pull { get; set; }

    /// <summary>
    /// Array of group names from which items can be pulled.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Pull"/> is set to <see cref="SortablePullMode.Groups"/>.
    /// </remarks>
    [Parameter]
    public string[]? PullGroups { get; set; }

    /// <summary>
    /// Factory function used to create a deep copy of an item when items are pulled in clone mode.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Pull"/> is set to <see cref="SortablePullMode.Clone"/>.
    /// </remarks>
    [Parameter]
    public Func<TItem, TItem>? CloneFunction { get; set; }

    /// <summary>
    /// Function used to determine if an item can be pulled from this list to another list.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Pull"/> is set to <see cref="SortablePullMode.Function"/>.
    /// This feature does <b>not</b> work when the application is running in anything
    /// other than <b>Blazor WebAssembly</b>.
    /// Sortable.js requires a synchronous JS-to-.NET call, which is not supported
    /// outside of WebAssembly (for example, in Blazor Server or Blazor Hybrid).
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Thrown when used in a non-WebAssembly environment.
    /// </exception>
    [Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
    public Predicate<SortableTransferContext<TItem>>? PullFunction
#pragma warning restore BL0007 // Component parameters should be auto properties
    {
        get => pullFunction;
        set
        {
            if (!OperatingSystem.IsBrowser())
                throw new NotSupportedException("PullFunction is only supported in Blazor WebAssembly");
            pullFunction = value;
        }
    }

    /// <summary>
    /// Mode for adding items to this Sortable component.
    /// </summary>
    [Parameter]
    public SortablePutMode? Put { get; set; }

    /// <summary>
    /// Array of group names from which items can be added.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Put"/> is set to <see cref="SortablePutMode.Groups"/>.
    /// </remarks>
    [Parameter]
    public string[]? PutGroups { get; set; }

    /// <summary>
    /// Custom function to determine whether an item can be added to this Sortable component.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Put"/> is set to <see cref="SortablePutMode.Function"/>.
    /// This feature does <b>not</b> work when the application is running in anything
    /// other than <b>Blazor WebAssembly</b>.
    /// Sortable.js requires a synchronous JS-to-.NET call, which is not supported
    /// outside of WebAssembly (for example, in Blazor Server or Blazor Hybrid).
    /// </remarks>
    /// <exception cref="NotSupportedException">
    /// Thrown when used in a non-WebAssembly environment.
    /// </exception>
    [Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
    public Predicate<SortableTransferContext<object>>? PutFunction
#pragma warning restore BL0007 // Component parameters should be auto properties
    {
        get => putFunction;
        set
        {
            if (!OperatingSystem.IsBrowser())
                throw new NotSupportedException("PutFunction is only supported in Blazor WebAssembly");
            putFunction = value;
        }
    }

    /// <summary>
    /// Dictionary of converters for transforming items from other SortableLists.
    /// </summary>
    /// <remarks>
    /// The key is the <c>Id</c> of another <see cref="Sortable{TItem}"/> that provides items,
    /// and the value is a function that converts an item from that list to the target <typeparamref name="TItem"/> type.
    /// This is used when items are dragged between lists with different data types.
    /// </remarks>
    [Parameter]
    public Func<SortableTransferContext<object>, TItem?>? ConvertFunction { get; set; }

    /// <summary>
    /// Enables or disables sorting of items within the list.
    /// </summary>
    [Parameter]
    public bool Sort { get; set; } = true;

    /// <summary>
    /// Time in milliseconds to define when the sorting should start. Unfortunately, due to browser restrictions, delaying is not possible on IE or Edge with native drag and drop.
    /// </summary>
    [Parameter]
    public int Delay { get; set; }

    /// <summary>
    /// Whether or not the delay should be applied only if the user is using touch (eg. on a mobile device). No delay will be applied in any other case.
    /// </summary>
    [Parameter]
    public bool DelayOnTouchOnly { get; set; }

    /// <summary>
    /// When the <see cref="Delay"/> option is set, some phones with very sensitive touch displays like the Samsung Galaxy S8 will fire unwanted touchmove events even when your finger is not moving, resulting in the sort not triggering.
    /// This option sets the minimum pointer movement that must occur before the delayed sorting is cancelled.
    /// Values between 3 to 5 are good.
    /// </summary>
    [Parameter]
    public int TouchStartThreshold { get; set; }

    /// <summary>
    /// Disables the Sortable component when set to true.
    /// When disabled, drag and drop operations are not allowed.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// Animation duration in milliseconds.
    /// </summary>
    [Parameter]
    public int Animation { get; set; } = 150;

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
    public Predicate<TItem>? DraggableSelector { get; set; }

    /// <summary>
    /// CSS class applied to items that can be dragged.
    /// </summary>
    /// <remarks>
    /// Used in conjunction with <see cref="DraggableSelector"/> to style draggable items.
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
    /// If set to true, the Fallback for non HTML5 Browser will be used, even if we are using an HTML5 Browser.
    /// This gives us the possibility to test the behaviour for older Browsers even in newer Browser, or make the Drag 'n Drop feel more consistent between Desktop, Mobile and old Browsers.
    /// On top of that, the Fallback always generates a copy of that DOM Element and appends the class fallbackClass defined in the options. This behaviour controls the look of this 'dragged' Element.
    /// </summary>
    [Parameter]
    public bool ForceFallback { get; set; } = true;

    /// <summary>
    /// CSS class for the element in fallback mode.
    /// </summary>
    [Parameter]
    public string FallbackClass { get; set; } = "sortable-fallback";

    /// <summary>
    /// Appends the cloned DOM Element into the Document's Body.
    /// </summary>
    [Parameter]
    public bool FallbackOnBody { get; set; }

    /// <summary>
    /// Emulates the native drag threshold. Specify in pixels how far the mouse should move before it's considered as a drag. Useful if the items are also clickable like in a list of links.
    /// When the user clicks inside a sortable element, it's not uncommon for your hand to move a little between the time you press and the time you release.
    /// Dragging only starts if you move the pointer past a certain tolerance, so that you don't accidentally start dragging every time you click.
    /// 3 to 5 are probably good values.
    /// </summary>
    [Parameter]
    public int FallbackTolerance { get; set; }

    ///// <summary>
    ///// Enables multi-drag functionality.
    ///// </summary>
    //[Parameter]
    //public bool MultiDrag { get; set; }

    ///// <summary>
    ///// CSS class for selected items in multi-drag mode.
    ///// </summary>
    //[Parameter]
    //public string SelectedClass { get; set; } = "sortable-selected";

    ///// <summary>
    ///// Key used to enable multi-drag selection.
    ///// </summary>
    ///// <remarks>
    ///// Default is "Control" key. Users must hold this key while clicking to select multiple items.
    ///// </remarks>
    //[Parameter]
    //public string? MultiDragKey { get; set; } = "Control";

    ///// <summary>
    ///// Prevents automatic deselection when clicking on selected items.
    ///// </summary>
    ///// <remarks>
    ///// When true, clicking on a selected item will not deselect it.
    ///// Useful for maintaining selection state during complex interactions.
    ///// </remarks>
    //[Parameter]
    //public bool AvoidImplicitDeselect { get; set; }

    ///// <summary>
    ///// Enables swap mode for dragging.
    ///// </summary>
    ///// <remarks>
    ///// When enabled, dragging an item over another item will swap their positions
    ///// instead of inserting the dragged item at the new position.
    ///// </remarks>
    //[Parameter]
    //public bool Swap { get; set; }

    ///// <summary>
    ///// CSS class applied to items during swap highlighting.
    ///// </summary>
    ///// <remarks>
    ///// Applied to items that would be swapped when <see cref="Swap"/> is enabled.
    ///// </remarks>
    //[Parameter]
    //public string SwapClass { get; set; } = "sortable-swap-highlight";

    /// <summary>
    /// Enables scrolling of the container during dragging.
    /// </summary>
    /// <remarks>
    /// When enabled, the container will scroll when dragging items near its edges.
    /// </remarks>
    [Parameter]
    public bool Scroll { get; set; } = true;

    /// <summary>
    /// Event that occurs when the order of items in the list is updated.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    /// </remarks>
    [Parameter]
    public Action<SortableEventArgs<TItem>>? OnUpdate { get; set; }

    /// <summary>
    /// Event that occurs when an item is added to the list.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    /// </remarks>
    [Parameter]
    public Action<SortableEventArgs<TItem>>? OnAdd { get; set; }

    /// <summary>
    /// Event that occurs when an item is removed from the list.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    /// </remarks>
    [Parameter]
    public Action<SortableEventArgs<TItem>>? OnRemove { get; set; }

    ///// <summary>
    ///// Event that occurs when an item is selected in multi-drag mode.
    ///// </summary>
    ///// <remarks>
    ///// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    ///// </remarks>
    //[Parameter]
    //public Action<TItem>? OnSelect { get; set; }

    ///// <summary>
    ///// Event that occurs when an item is deselected in multi-drag mode.
    ///// </summary>
    ///// <remarks>
    ///// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    ///// </remarks>
    //[Parameter]
    //public Action<TItem>? OnDeselect { get; set; }

    [Inject] private ISortableRegistry SortableRegistry { get; set; } = default!;
    [Inject] private IJSRuntime Js { get; set; } = default!;

    private Predicate<SortableTransferContext<TItem>>? pullFunction;
    private Predicate<SortableTransferContext<object>>? putFunction;

    private IJSObjectReference? jsModule;
    private DotNetObjectReference<Sortable<TItem>>? selfReference;

    private int draggedItemIndex = -1;
    private bool suppressNextRemove;

    /// <inheritdoc/>
    protected override sealed void OnInitialized()
    {
        switch (Pull)
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

        switch (Put)
        {
            case SortablePutMode.Groups:
                ArgumentNullException.ThrowIfNull(PutGroups);
                break;
            case SortablePutMode.Function:
                ArgumentNullException.ThrowIfNull(PutFunction);
                break;
        }
    }

    /// <inheritdoc/>
    protected override sealed async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            jsModule = await Js.InvokeAsync<IJSObjectReference>("import",
                $"./_content/BlazorSortable/js/blazor-sortable.js{SortableAssemblyInfo.VersionQuery}");
            selfReference = DotNetObjectReference.Create(this);
            await jsModule.InvokeVoidAsync("initSortable", Id, BuildOptions(), selfReference);

            SortableRegistry.Register(Id, this);
        }
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
                // Ignore - Blazor Server Circuit Disconnected
            }
        }

        // Dispose selfReference after JavaScript module
        selfReference?.Dispose();

        SortableRegistry.Unregister(Id);
    }

    private Dictionary<string, object> BuildOptions()
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

        var options = new Dictionary<string, object>
        {
            ["group"] = group,
            ["sort"] = Sort,
            ["delay"] = Delay,
            ["delayOnTouchOnly"] = DelayOnTouchOnly,
            ["touchStartThreshold"] = TouchStartThreshold,
            ["disabled"] = Disabled,
            ["animation"] = Animation,
            ["ghostClass"] = GhostClass,
            ["chosenClass"] = ChosenClass,
            ["dragClass"] = DragClass,
            ["swapThreshold"] = SwapThreshold,
            ["invertSwap"] = InvertSwap,
            ["invertedSwapThreshold"] = InvertedSwapThreshold,
            ["forceFallback"] = ForceFallback,
            ["fallbackClass"] = FallbackClass,
            ["fallbackOnBody"] = FallbackOnBody,
            ["fallbackTolerance"] = FallbackTolerance
        };

        if (!string.IsNullOrWhiteSpace(Handle))
            options["handle"] = Handle;

        if (!string.IsNullOrWhiteSpace(Filter))
            options["filter"] = Filter;

        if (DraggableSelector is not null)
            options["draggable"] = "." + DraggableClass;

        //if (MultiDrag)
        //{
        //    options["multiDrag"] = true;
        //    options["selectedClass"] = SelectedClass;
        //    options["avoidImplicitDeselect"] = AvoidImplicitDeselect;

        //    if (!string.IsNullOrWhiteSpace(MultiDragKey))
        //        options["multiDragKey"] = MultiDragKey;
        //}

        //if (Swap)
        //{
        //    options["swap"] = true;
        //    options["swapClass"] = SwapClass;
        //}

        options["scroll"] = Scroll;

        // Possible bug in OnSpill: item might be removed from the list even if removeOnSpill is false and revertOnSpill is true

        return options;
    }

    private object? GetPull()
    {
        return Pull switch
        {
            SortablePullMode.True => true,
            SortablePullMode.False => false,
            SortablePullMode.Groups => PullGroups,
            SortablePullMode.Clone => "clone",
            SortablePullMode.Function => "function",
            _ => null
        };
    }

    private object? GetPut()
    {
        return Put switch
        {
            SortablePutMode.True => true,
            SortablePutMode.False => false,
            SortablePutMode.Groups => PutGroups,
            SortablePutMode.Function => "function",
            _ => null
        };
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public void OnStartJs(int index)
    {
        draggedItemIndex = index;
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public void OnEndJs()
    {
        draggedItemIndex = -1;
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public bool OnPullJs(string toId)
    {
        var item = Items![draggedItemIndex];
        var to = SortableRegistry[toId]!;
        var ctx = new SortableTransferContext<TItem>(item, this, to);
        return PullFunction!(ctx);
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public bool OnPutJs(string fromId)
    {
        var from = SortableRegistry[fromId]!;
        var item = from[from.DraggedItemIndex];
        var ctx = new SortableTransferContext<object>(item, from, this);
        return PutFunction!(ctx);
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public void OnUpdateJs(int oldIndex, int newIndex)
    {
        var item = Items![oldIndex];

        // Sometimes SortableJS provides newIndex one greater than the last valid index
        if (Items.Count == 1 && newIndex == 1)
            newIndex = 0;

        var args = new SortableEventArgs<TItem>(item, this, oldIndex, this, newIndex);
        OnUpdate?.Invoke(args);
        if (args.Cancel)
            return;

        Items.RemoveAt(oldIndex);
        Items.Insert(newIndex, item);
        StateHasChanged();
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public void OnAddJs(string fromId, int oldIndex, int newIndex, bool isClone)
    {
        var from = SortableRegistry[fromId]!;
        from.SuppressNextRemove = !isClone;

        var sourceObject = from[oldIndex];

        TItem? item = default;
        if (ConvertFunction is not null)
        {
            item = ConvertFunction(new SortableTransferContext<object>(sourceObject, from, this));
        }
        else if (sourceObject is TItem sourceItem)
        {
            item = sourceItem;
        }
        if (item is null)
            return;

        var args = new SortableEventArgs<TItem>(item, from, oldIndex, this, newIndex, isClone);
        OnAdd?.Invoke(args);
        if (args.Cancel)
            return;

        from.SuppressNextRemove = false;

        if (Items is not null)
        {
            Items.Insert(newIndex, item);
            StateHasChanged();
        }
    }

    [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    public void OnRemoveJs(int oldIndex, string toId, int newIndex)
    {
        if (suppressNextRemove)
        {
            suppressNextRemove = false;
            return;
        }

        var item = Items![oldIndex];
        var to = SortableRegistry[toId]!;

        var args = new SortableEventArgs<TItem>(item, this, oldIndex, to, newIndex);
        OnRemove?.Invoke(args);
        if (args.Cancel)
            return;

        Items.RemoveAt(oldIndex);
        StateHasChanged();
    }

    //[JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    //public void OnSelectJs(int index)
    //{
    //    OnSelect?.Invoke(Items![index]);
    //}

    //[JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
    //public void OnDeselectJs(int index)
    //{
    //    OnDeselect?.Invoke(Items![index]);
    //}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    object ISortableList.this[int index]
    {
        get
        {
            var item = Items![index]!;
            return Pull == SortablePullMode.Clone ? CloneFunction!(item)! : item;
        }
    }

    int ISortableList.DraggedItemIndex => draggedItemIndex;

    bool ISortableList.SuppressNextRemove
    {
        get => suppressNextRemove;
        set => suppressNextRemove = value;
    }
}
