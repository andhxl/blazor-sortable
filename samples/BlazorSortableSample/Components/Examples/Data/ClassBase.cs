namespace BlazorSortableSample.Components.Examples.Data;

public abstract class ClassBase
{
    public int Value { get; set; }

    public override string ToString() => $"{GetType().Name}: {Value}";
}
