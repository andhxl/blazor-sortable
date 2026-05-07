namespace BlazorSortable;

/// <param name="Item">The item whose selection state changed.</param>
/// <param name="SelectedItems">The selected items after the selection change.</param>
public sealed record SortableSelectionEventArgs<TItem>(
    TItem Item,
    IReadOnlyList<TItem> SelectedItems)
    where TItem : notnull;
