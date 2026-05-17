namespace BlazorSortable;

/// <summary>
/// Provides identifying information about a Sortable component.
/// </summary>
/// <param name="Id">Unique identifier of the Sortable component.</param>
/// <param name="Group">Group name for interaction with other Sortable components.</param>
/// <remarks>
/// Represents a snapshot of the component identity and does not expose the component instance.
/// </remarks>
public sealed record SortableInfo(string Id, string Group);
