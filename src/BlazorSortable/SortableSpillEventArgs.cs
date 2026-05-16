namespace BlazorSortable;

/// <summary>
/// Represents event arguments for an item dropped outside a valid sortable target.
/// </summary>
/// <typeparam name="TItem">The type of the items.</typeparam>
/// <param name="Item">The primary item dropped outside a valid sortable target.</param>
/// <param name="Items">The items participating in a multi-drag spill operation. Empty for single-item operations.</param>
/// <param name="IsClone">Indicates whether the spill operation uses a cloned dragged item.</param>
public sealed record SortableSpillEventArgs<TItem>(
    TItem Item,
    IReadOnlyList<TItem> Items,
    bool IsClone)
    where TItem : notnull;
