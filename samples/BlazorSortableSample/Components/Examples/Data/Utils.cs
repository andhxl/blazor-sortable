namespace BlazorSortableSample.Components.Examples.Data;

public static class Utils
{
    public static IList<T> CreateItemList<T>(int count, int startValue = 1) where T : ClassBase, new()
    {
        var list = new List<T>();

        for (int i = 0; i < count; i++)
            list.Add(new T { Value = startValue++ });

        return list;
    }
}
