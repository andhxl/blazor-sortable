namespace BlazorSortable.Internal;

internal static class AssemblyVersionQuery
{
    public static string Value { get; } = Build();

    private static string Build()
    {
        var v = typeof(AssemblyVersionQuery).Assembly.GetName().Version;
        return v is null ? string.Empty : $"?v={v}";
    }
}
