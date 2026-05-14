namespace BlazorSortable;

public sealed record SortableSpillEventArgs<TItem>(
    TItem Item,
    IReadOnlyList<TItem> Items,
    bool IsClone)
    where TItem : notnull;
