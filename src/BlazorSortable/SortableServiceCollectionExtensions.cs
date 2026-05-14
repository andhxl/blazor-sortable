using BlazorSortable.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BlazorSortable;

/// <summary>
/// Extension methods for setting up BlazorSortable services in an <see cref="IServiceCollection" />.
/// </summary>
public static class SortableServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds BlazorSortable services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        public IServiceCollection AddSortable() =>
            services.AddSortable(static _ => { });

        /// <summary>
        /// Adds BlazorSortable services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="configureOptions">The <see cref="SortableOptions" /> configuration delegate.</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        public IServiceCollection AddSortable(Action<SortableOptions> configureOptions)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configureOptions);

            services.TryAddScoped<SortableRegistry>();
            services.Configure(configureOptions);

            return services;
        }

        /// <summary>
        /// Adds BlazorSortable services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="options">The <see cref="SortableOptions" /> instance to use.</param>
        /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
        public IServiceCollection AddSortable(SortableOptions options)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(options);

            services.TryAddScoped<SortableRegistry>();
            services.TryAddSingleton(Options.Create(options));

            return services;
        }
    }
}
