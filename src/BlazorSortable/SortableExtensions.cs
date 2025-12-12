using BlazorSortable.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSortable;

/// <summary>
/// Provides extension methods.
/// </summary>
public static class SortableExtensions
{
    /// <summary>
    /// Adds the necessary services for Sortable functionality to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddSortable(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<ISortableRegistry, SortableRegistry>();

        return services;
    }
}
