using Microsoft.AspNetCore.Components;

namespace BlazorSortableDemo.Components.Examples;

public static class ExampleCatalog
{
    public const string RouteTemplate = "/examples/{Id}";

    public static string GetPath(string id) =>
        RouteTemplate.Replace("{Id}", id);

    public static readonly (Type Type, string Id, string Title)[] All =
    [
        Example<SimpleListExample>(),
        Example<HandleExample>(),
        Example<SharedListsExample>(),
        Example<MultiDragExample>(),
        Example<SwapExample>(),
        Example<AdvancedExample>(),
        Example<DropZoneExample>(),
    ];

    public static (Type Type, string Id, string Title)? Find(string? id)
    {
        if (id is null)
            return null;

        foreach (var example in All)
        {
            if (example.Id == id)
                return example;
        }

        return null;
    }

    private static (Type Type, string Id, string Title) Example<T>()
        where T : IComponent, IExample
    {
        return (typeof(T), T.Id, T.Title);
    }
}
