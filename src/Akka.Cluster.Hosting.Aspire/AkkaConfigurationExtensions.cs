using Akka.Hosting;
using Akka.Hosting.Aspire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akka.Cluster.Hosting.Aspire;

public static class AkkaConfigurationExtensions
{
    public static AkkaConfigurationBuilder WithClustering(this AkkaConfigurationBuilder builder, IServiceProvider provider, string? sectionName = null, Action<ClusterOptions>? configure = null)
    {
        sectionName ??= AkkaConfigurationStrings.GetAkkaClusterString();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var clusterOptions = configuration.GetSection(sectionName).Get<ClusterOptions>()!;
        configure?.Invoke(clusterOptions);
        return builder.WithClustering(clusterOptions);
    }
}