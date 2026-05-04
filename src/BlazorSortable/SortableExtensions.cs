using BlazorSortable.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSortable;

/// <summary>
/// Provides service registration extensions for BlazorSortable.
/// </summary>
public static class SortableExtensions
{
    /// <summary>
    /// Adds Sortable services to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is null.
    /// </exception>
    public static IServiceCollection AddSortable(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<SortableRegistry>();

        return services;
    }
}
