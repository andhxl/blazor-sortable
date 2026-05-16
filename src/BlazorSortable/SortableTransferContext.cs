namespace BlazorSortable;

/// <summary>
/// Represents the context for transferring items between sortable components.
/// </summary>
/// <typeparam name="TItem">The type of the items.</typeparam>
/// <param name="Item">The primary item being transferred.</param>
/// <param name="Items">Items associated with the transfer when a collection is available.</param>
/// <param name="From">Source sortable information.</param>
/// <param name="To">Target sortable information.</param>
public sealed record SortableTransferContext<TItem>(
    TItem Item,
    IReadOnlyList<TItem> Items,
    SortableInfo From,
    SortableInfo To)
    where TItem : notnull;
