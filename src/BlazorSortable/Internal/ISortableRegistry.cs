namespace BlazorSortable.Internal;

internal interface ISortableRegistry
{
    void Register(string id, ISortableList sortable);

    void Unregister(string id);

    ISortableList this[string id] { get; }
}
