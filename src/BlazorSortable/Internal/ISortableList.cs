namespace BlazorSortable.Internal;

internal interface ISortableList : ISortableInfo
{
    object GetTransferItem(int index);

    bool TrySetPendingSwapItem(object item, SortableInfo from, SortableInfo to);

    int DraggedItemIndex { get; }

    IReadOnlyList<int> DraggedItemIndexes { get; }

    bool ShouldSkipNextRemove { get; set; }
}
