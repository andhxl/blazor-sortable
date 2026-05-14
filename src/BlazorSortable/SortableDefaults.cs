namespace BlazorSortable;

/// <summary>
/// Provides default behavior options for Sortable components.
/// </summary>
public sealed class SortableDefaults
{
    /// <summary>
    /// Forces SortableJS to use fallback drag behavior instead of native HTML5 drag and drop.
    /// </summary>
    public bool ForceFallback { get; set; } = true;

    /// <summary>
    /// Appends the fallback clone element to the document body.
    /// </summary>
    public bool FallbackOnBody { get; set; }

    /// <summary>
    /// Minimum pointer movement, in pixels, before fallback dragging starts.
    /// </summary>
    public int FallbackTolerance { get; set; } = 3; // So that we can multi-drag select items on mobile

    /// <summary>
    /// Animation duration in milliseconds.
    /// </summary>
    public int Animation { get; set; } = 150;

    /// <summary>
    /// Time in milliseconds before sorting starts.
    /// </summary>
    public int Delay { get; set; } = 150;

    /// <summary>
    /// Applies <see cref="Delay"/> only for touch input.
    /// </summary>
    public bool DelayOnTouchOnly { get; set; } = true;

    /// <summary>
    /// Minimum touch movement, in pixels, before delayed sorting is cancelled.
    /// </summary>
    public int TouchStartThreshold { get; set; } = 4;

    public int EmptyInsertThreshold { get; set; } = 5;

    /// <summary>
    /// Enables automatic scrolling while dragging near scroll container edges.
    /// </summary>
    public bool Scroll { get; set; } = true;

    /// <summary>
    /// Reverts the dragged item when it is dropped outside a valid sortable target.
    /// </summary>
    public bool RevertOnSpill { get; set; }

    /// <summary>
    /// Key used to enable multi-drag selection.
    /// </summary>
    public string MultiDragKey { get; set; } = "Control";
}
