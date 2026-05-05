namespace BlazorSortable;

/// <summary>
/// Provides options for configuring BlazorSortable.
/// </summary>
public sealed class SortableOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the BlazorSortable stylesheet is loaded automatically.
    /// </summary>
    /// <remarks>
    /// Set this value to <see langword="false" /> to include the stylesheet manually or omit it entirely.
    /// </remarks>
    public bool LoadStylesheet { get; set; } = true;
}
