namespace BlazorSortableSample.Components.Examples.Data;

public static class Utils
{
    public static IList<TItem> CreateItemList<TItem>(int count, int startValue = 1)
        where TItem : ClassBase, new()
    {
        var list = new List<TItem>(count);

        for (int i = 0; i < count; i++)
            list.Add(new TItem { Value = startValue++ });

        return list;
    }
}
