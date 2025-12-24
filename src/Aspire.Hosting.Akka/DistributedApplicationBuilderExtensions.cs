using System.Collections.Generic;
using System.Linq;
using Aspire.Hosting.Akka;
using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public static class DistributedApplicationBuilderExtensions
{
    public static IResourceBuilder<AkkaClusterResource> AddAkkaCluster(this IDistributedApplicationBuilder builder,
        [ResourceName] string name, string? actorSystemName = null)
    {
        actorSystemName ??= name;
        return builder.AddResource(new AkkaClusterResource(name, actorSystemName));
    }

    public static IResourceBuilder<AkkaClusterResource> AddAkkaClusterWithLighthouse(
        this IDistributedApplicationBuilder builder, [ResourceName] string name, string? actorSystemName = null,
        int? clusterPort = null,
        int? pbmPort = null)
    {
        actorSystemName ??= name;
        var clusterResource = builder.AddAkkaCluster(name, actorSystemName);
        clusterResource.WithLighthouse(actorSystemName, clusterPort, pbmPort);
        return clusterResource;
    }

    public static IDistributedApplicationBuilder FinalizeAkkaCluster(this IDistributedApplicationBuilder builder)
    {
        foreach (var node in builder.GetNodes())
        {
            var seedNodes = builder.GetSeedNodeExpressions();
            for (var i = 0; i < seedNodes.Count; i++)
            {
                var seedNode = seedNodes[i];
                var nodeBuilder = builder.CreateResourceBuilder(node);
                nodeBuilder.WithEnvironment(context =>
                    {
                        context.EnvironmentVariables[$"akka__cluster__seed-nodes__{i}"] = seedNode;
                    })
                    .WithReferenceRelationship(seedNode);
            }
        }

        return builder;
    }

    internal static List<ReferenceExpression> GetSeedNodeExpressions(this IDistributedApplicationBuilder builder)
    {
        var clusterResource = builder.Resources.OfType<AkkaClusterResource>().Single();
        var seedNodes = builder.GetSeedNodes();
        return seedNodes.Values.Select(reference =>
                ReferenceExpression.Create(
                    $"{reference.Scheme}://{clusterResource.ActorSystemName}@{reference.Property(EndpointProperty.HostAndPort)}"))
            .ToList();
    }

    internal static IDictionary<string, EndpointReference> GetSeedNodes(this IDistributedApplicationBuilder builder)
    {
        return builder.Resources
            .OfType<IResourceWithEndpoints>()
            .SelectMany(r =>
                r.Annotations
                    .OfType<AkkaSeedNodeAnnotation>()
                    .Select(a => (Resource: r, Annotation: a)))
            .ToDictionary(key => key.Resource.Name, value => value.Resource.GetEndpoint(value.Annotation.EndpointName));
    }

    internal static List<IResourceWithEnvironment> GetNodes(this IDistributedApplicationBuilder builder)
    {
        return builder.Resources
            .OfType<IResourceWithEnvironment>()
            .Where(r => r.Annotations.OfType<AkkaNodeAnnotation>().Any())
            .Select(x => x)
            .ToList();
    }
}