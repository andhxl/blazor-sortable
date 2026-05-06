namespace BlazorSortable;

/// <summary>
/// Provides options for configuring BlazorSortable.
/// </summary>
public sealed class SortableOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the bundled SortableJS library is loaded automatically.
    /// </summary>
    /// <remarks>
    /// Set this value to <see langword="false" /> to load SortableJS yourself.
    /// </remarks>
    public bool AutoLoadSortableJs { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the bundled BlazorSortable stylesheet is loaded automatically.
    /// </summary>
    /// <remarks>
    /// Set this value to <see langword="false" /> to provide your own styles or omit the stylesheet.
    /// </remarks>
    public bool AutoLoadStylesheet { get; set; } = true;
}
