using System.Linq;
using Aspire.Hosting.Akka;
using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public static class ResourceBuilderExtensions
{
    private const int ClusterContainerPort = 4053;
    private const int PbmContainerPort = 9011;
    internal const string AkkaEndpointName = "akka";

    public static IResourceBuilder<LighthouseResource> WithLighthouse<T>(this IResourceBuilder<T> builder,
        string actorSystemName,
        int? clusterPort = null,
        int? pbmPort = null) where T : AkkaClusterResource
    {
        var lighthouseResource = new LighthouseResource("Lighthouse");
        return builder.ApplicationBuilder.AddResource(lighthouseResource)
            .WithImage(LighthouseContainerImageTags.Image, LighthouseContainerImageTags.Tag)
            .WithImageRegistry(LighthouseContainerImageTags.Registry)
            .WithEndpoint(port: clusterPort, targetPort: ClusterContainerPort, name: LighthouseResource.TcpEndpointName)
            .WithEndpoint(port: pbmPort, PbmContainerPort, name: LighthouseResource.PbmEndpointName)
            .WithAnnotation(new AkkaSeedNodeAnnotation(LighthouseResource.TcpEndpointName))
            .WithAnnotation(new AkkaNodeAnnotation())
            .WithEnvironment(context =>
            {
                var tcpEndpoint = lighthouseResource.TcpEndpoint;
                context.EnvironmentVariables[LighthouseResource.ActorSystemNameEnvName] = actorSystemName;
                context.EnvironmentVariables[LighthouseResource.ClusterIpEnvName] =
                    tcpEndpoint.Property(EndpointProperty.Host);
                context.EnvironmentVariables[LighthouseResource.ClusterPortEnvName] =
                    tcpEndpoint.Property(EndpointProperty.Port);
            });
    }

    public static IResourceBuilder<T> AsSeedNode<T>(this IResourceBuilder<T> builder, int? port = null,
        int? targetPort = null, string endpointName = "akka") where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        builder.WithAnnotation(new AkkaSeedNodeAnnotation(endpointName));
        return builder.AsAkkaNode(port, targetPort);
    }

    public static IResourceBuilder<T> AsAkkaNode<T>(this IResourceBuilder<T> builder, int? port = null,
        int? targetPort = null)
        where T : IResourceWithEndpoints, IResourceWithEnvironment
    {
        var akkaEndpointReference = new EndpointReference(builder.Resource, AkkaEndpointName);
        return builder
            .WithEndpoint(port: port, targetPort: targetPort, scheme: "akka.tcp",
                env: "akka__remote__dot-netty__tcp__port", name: AkkaEndpointName)
            .WithAnnotation(new AkkaNodeAnnotation())
            .WithEnvironment(context =>
            {
                if (context.Resource.TryGetEndpoints(out var endpoints) &&
                    endpoints.Any(x => x.Name == AkkaEndpointName))
                {
                    context.EnvironmentVariables["akka__remote__dot-netty__tcp__hostname"] =
                        akkaEndpointReference.Property(EndpointProperty.Host);
                }
            });
    }
}