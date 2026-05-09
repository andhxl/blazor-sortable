namespace BlazorSortable.Internal;

internal static class AssemblyVersion
{
    public static string Value { get; } =
        typeof(AssemblyVersion).Assembly.GetName().Version!.ToString(3);
}
