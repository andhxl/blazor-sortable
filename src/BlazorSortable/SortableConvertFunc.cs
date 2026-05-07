namespace BlazorSortable;

/// <summary>
/// Represents a method that attempts to convert a sortable transfer context to a strongly typed item.
/// </summary>
/// <remarks>Use this delegate to define custom conversion logic when transferring sortable data between
/// components or layers. The method should not throw exceptions for conversion failures; instead, it should return
/// false and set the out parameter to its default value.</remarks>
/// <typeparam name="TItem">The type of item to convert to. Must be a non-nullable type.</typeparam>
/// <param name="context">The sortable transfer context containing the data to convert.</param>
/// <param name="item">When this method returns, contains the converted item if the conversion succeeded; otherwise, the default value for
/// the type.</param>
/// <returns>true if the conversion was successful; otherwise, false.</returns>
public delegate bool SortableConvertFunc<TItem>(
    SortableTransferContext<object> context,
    out TItem item)
    where TItem : notnull;
