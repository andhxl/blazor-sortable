namespace BlazorSortableDemo.Pages.Examples;

public interface IExample
{
    static abstract string Id { get; }
    static abstract string Title { get; }
    static abstract string Code { get; }
}
