namespace BlazorSortable;

/// <summary>
/// Defines the operation that changed a Sortable component's items.
/// </summary>
public enum SortableChangeOperation
{
    /// <summary>
    /// Items were reordered within the same Sortable component.
    /// </summary>
    Update,

    /// <summary>
    /// Items were accepted by a Sortable component.
    /// </summary>
    Add,

    /// <summary>
    /// Items were removed from a Sortable component.
    /// </summary>
    Remove
}
