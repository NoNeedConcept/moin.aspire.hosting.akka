using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Aspire.Hosting.Akka;

public static class ServiceCollectionExtensions
{
    internal static IServiceCollection TryAddHostedService<THostedService>(this IServiceCollection services)
        where THostedService : class, IHostedService
    {
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, THostedService>());
        return services;
    }
}