using Akka.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akka.Cluster.Hosting;

public static class AkkaConfigurationExtensions
{
    public static AkkaConfigurationBuilder WithClustering(this AkkaConfigurationBuilder builder,
        IServiceProvider provider, string sectionName = "akka:cluster", Action<ClusterOptions>? configure = null)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var clusterSection = configuration.GetSection(sectionName);
        var clusterOptions = new ClusterOptions
        {
            SeedNodes = clusterSection.GetSection("seed-nodes").Get<string[]>(),
        };
        configure?.Invoke(clusterOptions);
        return builder.WithClustering(clusterOptions);
    }
}