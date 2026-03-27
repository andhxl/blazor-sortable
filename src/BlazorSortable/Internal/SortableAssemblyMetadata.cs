namespace BlazorSortable.Internal;

internal static class SortableAssemblyMetadata
{
    public static string VersionQuery { get; } = BuildVersionQuery();

    private static string BuildVersionQuery()
    {
        var v = typeof(SortableAssemblyMetadata).Assembly.GetName().Version;
        return v is null ? string.Empty : $"?v={v}";
    }
}
