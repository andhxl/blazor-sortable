using System.Collections.Concurrent;

namespace BlazorSortable.Internal;

internal sealed class SortableRegistry : ISortableRegistry
{
    private readonly ConcurrentDictionary<string, ISortableList> _sortables = [];

    public void Register(string id, ISortableList sortable)
    {
        if (!_sortables.TryAdd(id, sortable))
            throw new InvalidOperationException($"Sortable with ID '{id}' is already registered.");
    }

    public void Unregister(string id) => _sortables.TryRemove(id, out _);

    public ISortableList this[string id]
    {
        get
        {
            if (_sortables.TryGetValue(id, out var sortable))
                return sortable;

            throw new KeyNotFoundException($"Sortable with ID '{id}' is not registered.");
        }
    }
}
