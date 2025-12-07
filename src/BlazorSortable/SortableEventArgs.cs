namespace BlazorSortable;

/// <summary>
/// Represents event arguments for operations in Sortable components.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
/// <param name="Item">The item participating in the operation.</param>
/// <param name="From">Source sortable.</param>
/// <param name="OldIndex">The previous index of the item in the source sortable.</param>
/// <param name="To">Target sortable.</param>
/// <param name="NewIndex">The new index of the item in the target sortable.</param>
/// <param name="IsClone">Flag indicating whether the item is a clone.</param>
public record SortableEventArgs<TItem>(TItem Item, ISortableInfo From, int OldIndex, ISortableInfo To, int NewIndex, bool IsClone = false)
{
    /// <summary>
    /// Gets or sets a value indicating whether the current operation should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }
}
