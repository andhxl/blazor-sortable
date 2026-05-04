namespace BlazorSortable;

/// <summary>
/// Defines how items can be pulled from a Sortable component during drag-and-drop operations.
/// </summary>
public enum SortablePullMode
{
    /// <summary>
    /// Allows pulling items.
    /// </summary>
    True,

    /// <summary>
    /// Prohibits pulling items.
    /// </summary>
    False,

    /// <summary>
    /// Allows pulling items only into specified target groups.
    /// Requires setting the <see cref="Sortable{TItem}.PullGroups"/> parameter.
    /// </summary>
    Groups,

    /// <summary>
    /// Creates a clone of the item when dragging.
    /// Requires setting the <see cref="Sortable{TItem}.CloneFunction"/> parameter.
    /// </summary>
    Clone,

    /// <summary>
    /// Uses a custom function to determine whether items can be pulled.
    /// Requires setting the <see cref="Sortable{TItem}.PullFunction"/> parameter.
    /// </summary>
    Function
}
