namespace BlazorSortable;

/// <summary>
/// Represents event arguments for a multi-drag selection change.
/// </summary>
/// <typeparam name="TItem">The type of the items.</typeparam>
/// <param name="Item">The item whose selection state changed.</param>
/// <param name="SelectedItems">The selected items after the selection change.</param>
public sealed record SortableSelectionEventArgs<TItem>(
    TItem Item,
    IReadOnlyList<TItem> SelectedItems)
    where TItem : notnull;
