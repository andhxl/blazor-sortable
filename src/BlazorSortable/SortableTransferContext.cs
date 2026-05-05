namespace BlazorSortable;

/// <summary>
/// Represents the context for transferring an item between sortable components.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <param name="Item">The item being transferred.</param>
/// <param name="From">Source sortable information.</param>
/// <param name="To">Target sortable information.</param>
public sealed record SortableTransferContext<TItem>(
    TItem Item,
    SortableInfo From,
    SortableInfo To)
    where TItem : notnull;
