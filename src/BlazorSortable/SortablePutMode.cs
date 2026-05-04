namespace BlazorSortable;

/// <summary>
/// Defines how a Sortable component accepts items during drag-and-drop operations.
/// </summary>
public enum SortablePutMode
{
    /// <summary>
    /// Allows accepting items.
    /// </summary>
    True,

    /// <summary>
    /// Prohibits accepting items.
    /// </summary>
    False,

    /// <summary>
    /// Allows accepting items only from specified source groups.
    /// Requires setting the <see cref="Sortable{TItem}.PutGroups"/> parameter.
    /// </summary>
    Groups,

    /// <summary>
    /// Uses a custom function to determine whether items can be accepted.
    /// Requires setting the <see cref="Sortable{TItem}.PutFunction"/> parameter.
    /// </summary>
    Function
}
