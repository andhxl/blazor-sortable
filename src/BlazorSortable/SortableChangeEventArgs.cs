namespace BlazorSortable;

public sealed record SortableChangeEventArgs<TItem>(
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
