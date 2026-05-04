namespace BlazorSortable;

/// <summary>
/// Represents the context of transferring an item between sortable components.
/// </summary>
/// <typeparam name="TItem">The type of the item being transferred.</typeparam>
/// <param name="Item">The item being transferred between sortable components.</param>
/// <param name="From">The source sortable component.</param>
/// <param name="To">The target sortable component.</param>
public sealed record SortableTransferContext<TItem>(
    TItem Item,
    ISortableInfo From,
    ISortableInfo To)
    where TItem : notnull;
