using Microsoft.AspNetCore.Components;

namespace BlazorSortableDemo.Pages.Examples;

public static class ExampleCatalog
{
    public const string RouteTemplate = "/examples/{Id}";

    public static string GetRelativePath(string id) =>
        RouteTemplate.Replace("{Id}", id).TrimStart('/');

    public static readonly ExampleDefinition[] All =
    [
        Example<SimpleListExample>(),
        Example<HandleExample>(),
        Example<SharedListsExample>(),
        Example<MultiDragExample>(),
        Example<SwapExample>(),
        Example<AdvancedExample>(),
        Example<DropZoneExample>(),
    ];

    public static ExampleDefinition? Find(string? id)
    {
        if (id is null) return null;

        foreach (var example in All)
        {
            if (example.Id == id)
                return example;
        }

        return null;
    }

    private static ExampleDefinition Example<T>()
        where T : IComponent, IExample
    {
        return new(typeof(T), T.Id, T.Title, T.Code);
    }
}
