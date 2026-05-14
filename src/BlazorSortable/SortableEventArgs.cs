namespace BlazorSortable;

/// <summary>
/// Represents event arguments for a sortable operation.
/// </summary>
/// <typeparam name="TItem">The type of the items.</typeparam>
/// <param name="Item">The primary item participating in the operation.</param>
/// <param name="Items">All items participating in the operation. Contains one item for a single-item operation.</param>
/// <param name="From">Source sortable information.</param>
/// <param name="OldIndex">The previous index of the primary item in the source sortable.</param>
/// <param name="OldIndexes">The previous indexes of all items in the source sortable. Uses the same order as <paramref name="Items"/>.</param>
/// <param name="To">Target sortable information.</param>
/// <param name="NewIndex">The new index of the primary item in the target sortable.</param>
/// <param name="NewIndexes">The new indexes of all items in the target sortable. Uses the same order as <paramref name="Items"/>.</param>
/// <param name="IsClone">Indicates whether the operation uses cloned items.</param>
public sealed record SortableEventArgs<TItem>(
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
