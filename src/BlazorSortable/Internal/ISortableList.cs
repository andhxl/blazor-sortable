namespace BlazorSortable.Internal;

internal interface ISortableList : ISortableInfo
{
    object GetTransferItem(int index);

    int DraggedItemIndex { get; }

    bool ShouldSkipNextRemove { get; set; }
}
