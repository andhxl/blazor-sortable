namespace BlazorSortable;

/// <summary>
/// Represents event arguments for a sortable operation.
/// </summary>
/// <typeparam name="TItem">The type of the items.</typeparam>
/// <param name="Operation">The operation that changed the list.</param>
/// <param name="Item">The primary item participating in the operation.</param>
/// <param name="Items">The items participating in a multi-drag operation. Empty for single-item operations.</param>
/// <param name="From">Source sortable information.</param>
/// <param name="OldIndex">The previous index of the primary item in the source sortable.</param>
/// <param name="OldIndexes">The previous indexes of the items in a multi-drag operation. Empty for single-item operations. Uses the same order as <paramref name="Items"/>.</param>
/// <param name="To">Target sortable information.</param>
/// <param name="NewIndex">The new index of the primary item in the target sortable.</param>
/// <param name="NewIndexes">The new indexes of the items in a multi-drag operation. Empty for single-item operations. Uses the same order as <paramref name="Items"/>.</param>
/// <param name="IsClone">Indicates whether the operation uses a cloned dragged item.</param>
/// <param name="IsSwap">Indicates whether the dragged item was swapped with another item. The target swap item is not exposed separately.</param>
public sealed record SortableEventArgs<TItem>(
    SortableChangeOperation Operation,
    TItem Item,
    IReadOnlyList<TItem> Items,
    SortableInfo From,
    int OldIndex,
    IReadOnlyList<int> OldIndexes,
    SortableInfo To,
    int NewIndex,
    IReadOnlyList<int> NewIndexes,
    bool IsClone,
    bool IsSwap)
    where TItem : notnull;
