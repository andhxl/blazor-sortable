namespace BlazorSortableDemo.Pages.Examples.Data;

public static class Utils
{
    public static IList<TItem> CreateItemList<TItem>(int count, int offset = 0)
        where TItem : ClassBase, new()
    {
        var list = new List<TItem>(count);

        for (int i = 0; i < count; i++)
        {
            var value = offset + i + 1;
            list.Add(new TItem { Id = value, Value = value });
        }

        return list;
    }
}
