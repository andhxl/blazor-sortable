namespace BlazorSortable.Internal;

/// <summary>
/// Provides identifying information about a Sortable component.
/// </summary>
internal interface ISortableInfo
{
    /// <summary>
    /// Unique identifier of the Sortable component.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Group name for interaction with other Sortable components.
    /// </summary>
    string Group { get; }
}
