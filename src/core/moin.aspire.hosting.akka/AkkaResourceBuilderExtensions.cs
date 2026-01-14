using System;
using Aspire.Hosting.ApplicationModel;
using moin.aspire.hosting.akka;
using Servus.Core.Network;

namespace Aspire.Hosting;

public static class AkkaResourceBuilderExtensions
{
    private const int AkkaContainerPort = 4053;
    private const int PbmContainerPort = 9011;
    internal const string AkkaEndpointName = "akka";

    public static IAkkaResourceBuilder WithCustomEnvNames(this AkkaResourceBuilder builder,
        Action<AkkaEnvNameBuilder>? configure = null)
    {
        var envNameBuilder = new AkkaEnvNameBuilder();
        configure?.Invoke(envNameBuilder);
        builder.SeedNodeEnvNameOptionDefault = envNameBuilder.SeedNodeEnvNameOptions;
        builder.HostnameEnvNameDefault = envNameBuilder.HostnameEnvName;
        builder.PortEnvNameDefault = envNameBuilder.PortEnvName;
        return builder;
    }

    public static IAkkaResourceBuilder WithLighthouse(this IAkkaResourceBuilder builder, int replicas,
        int[]? clusterPorts = null, int[]? pbmPorts = null)
    {
        for (var i = 0; i < replicas; i++)
        {
            builder.WithLighthouse(
                name: $"lighthouse-{i}",
                clusterPort: clusterPorts?[i],
                pbmPort: pbmPorts?[i]);
        }

        return builder;
    }

    public static IAkkaResourceBuilder WithLighthouse(this IAkkaResourceBuilder builder,
        [ResourceName] string name = "lighthouse",
        int? clusterPort = null,
        int? pbmPort = null,
        int? targetClusterPort = null,
        int? targetPbmPort = null,
        Action<IResourceBuilder<LighthouseResource>>? configure = null)
    {
        var lighthouseResource = new LighthouseResource(name);
        var lighthouseResourceBuilder = builder.AddResource(lighthouseResource)
            .WithImage(LighthouseContainerImageTags.Image, LighthouseContainerImageTags.Tag)
            .WithImageRegistry(LighthouseContainerImageTags.Registry)
            .WithEndpoint(
                port: clusterPort ?? PortFinder.FindFreeLocalPort(),
                targetPort: targetClusterPort ?? AkkaContainerPort,
                scheme: "akka.tcp",
                name: AkkaEndpointName,
                env: LighthouseResource.ClusterPortEnvName,
                isProxied: false)
            .WithEndpoint(
                port: pbmPort ?? PortFinder.FindFreeLocalPort(),
                targetPort: targetPbmPort ?? PbmContainerPort,
                name: LighthouseResource.PbmEndpointName,
                isProxied: false)
            .WithAnnotation(new AkkaClusterAnnotation(builder.SystemName))
            .WithAnnotation(new LighthouseAnnotation())
            .WithAnnotation(new AkkaSeedNodeAnnotation())
            .WithAnnotation(new AkkaNodeAnnotation(o =>
            {
                o.SeedNodeEnvName = LighthouseResource.ClusterSeedsEnvName;
                o.Mode = EnvValueMode.SingleValue;
            }))
            .WithEnvironment(LighthouseResource.ActorSystemNameEnvName, builder.SystemName)
            .WithEnvironment(context =>
            {
                var endpoint = lighthouseResource.GetEndpoint(AkkaEndpointName);
                context.EnvironmentVariables[LighthouseResource.ClusterIpEnvName] =
                    ReferenceExpression.Create($"{endpoint.Property(EndpointProperty.Host)}");
            });
        configure?.Invoke(lighthouseResourceBuilder);
        return builder;
    }

    public static IAkkaResourceBuilder WithSeedNode<T>(this IAkkaResourceBuilder builder,
        IResourceBuilder<T> resource, int targetPort, string endpointName = AkkaEndpointName)
        where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        resource.WithAnnotation(new AkkaSeedNodeAnnotation(endpointName));
        return builder.WithNode(resource, targetPort, endpointName);
    }

    public static IAkkaResourceBuilder WithNode<T>(this IAkkaResourceBuilder builder,
        IResourceBuilder<T> resource,
        int targetPort,
        [EndpointName] string endpointName = AkkaEndpointName,
        Action<AkkaEnvNameBuilder>? configure = null)
        where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        var envNameBuilder = builder.CreateEnvNameBuilder();
        configure?.Invoke(envNameBuilder);

        resource
            .WithAnnotation(new AkkaClusterAnnotation(builder.SystemName), ResourceAnnotationMutationBehavior.Replace)
            .WithAnnotation(new AkkaNodeAnnotation(envNameBuilder.SeedNodeEnvNameOptions),
                ResourceAnnotationMutationBehavior.Replace)
            .WithEndpoint(targetPort: targetPort, scheme: "akka.tcp", env: envNameBuilder.PortEnvName,
                name: endpointName, isProxied: false)
            .WithEnvironment(context =>
            {
                var endpoint = resource.GetEndpoint(endpointName);
                context.EnvironmentVariables[envNameBuilder.HostnameEnvName] =
                    ReferenceExpression.Create($"{endpoint.Property(EndpointProperty.Host)}");
            });
        return builder;
    }
}