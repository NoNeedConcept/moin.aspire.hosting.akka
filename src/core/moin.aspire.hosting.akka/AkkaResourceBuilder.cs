using System.Linq;
using Aspire.Hosting.Akka;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Health;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Hosting;

public class AkkaResourceBuilder(string name, string systemName, IDistributedApplicationBuilder builder)
    : IAkkaResourceBuilder
{
    public IDistributedApplicationBuilder DistributedApplicationBuilder => builder;
    public string SystemName => systemName;

    public IResourceBuilder<T> AddResource<T>(T instance) where T : IResource
        => DistributedApplicationBuilder.AddResource(instance);

    internal IResourceBuilder<AkkaResource> Build()
    {
        var seedNodeResources = builder.GetSeedNodes(SystemName).Keys.ToArray();
        var nodes = builder.GetNodes(SystemName).ToArray();
        var akkaResource = new AkkaResource(name, systemName, seedNodeResources, nodes);
        var akkaResourceBuilder = AddResource(akkaResource)
            .WithIconName("ServerMultiple")
            .WithInitialState(new CustomResourceSnapshot
            {
                State = new ResourceStateSnapshot("Starting", KnownResourceStateStyles.Info),
                ResourceType = akkaResource.GetType().Name,
                IsHidden = true,
                Properties = []
            })
            .ExcludeFromManifest();

        foreach (var node in nodes)
        {
            var nodeBuilder = builder.CreateResourceBuilder(node);

            var isLighthouse = node.HasAnnotationOfType<LighthouseAnnotation>();
            if (isLighthouse)
            {
                nodeBuilder.WithEnvironment(name: LighthouseResource.ClusterSeedsEnvName,
                    value: builder.GetSeedNodesForLighthouse(systemName));
                continue;
            }

            var seedNodes = builder.GetSeedNodeAddresse(SystemName);
            for (var i = 0; i < seedNodes.Length; i++)
            {
                nodeBuilder
                    .WithEnvironment(name: $"akka__cluster__seed-nodes__{i}", value: seedNodes[i])
                    .WithReferenceRelationship(seedNodeResources[i]);
            }
        }

        builder.Services.AddHealthChecks()
            .Add(new HealthCheckRegistration(
                name: $"akka-cluster-{systemName}",
                factory: sp => new AkkaAggregateHealthCheck(
                    sp.GetRequiredService<ResourceNotificationService>(),
                    akkaResource),
                failureStatus: HealthStatus.Unhealthy,
                tags: null));

        builder.Services.TryAddHostedService<AkkaResourceHealthMonitoringService>();

        return akkaResourceBuilder;
    }
}