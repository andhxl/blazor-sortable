using System.Collections.Concurrent;

namespace BlazorSortable.Internal;

internal class SortableRegistry : ISortableRegistry
{
    private readonly ConcurrentDictionary<string, ISortableList> _sortables = [];

    public void Register(string id, ISortableList sortable)
    {
        if (!_sortables.TryAdd(id, sortable))
        {
            throw new InvalidOperationException($"A Sortable with ID '{id}' is already registered");
        }
    }

    public void Unregister(string id)
    {
        _sortables.TryRemove(id, out _);
    }

    public ISortableList? this[string id]
    {
        get
        {
            _sortables.TryGetValue(id, out var sortable);
            return sortable;
        }
    }
}
