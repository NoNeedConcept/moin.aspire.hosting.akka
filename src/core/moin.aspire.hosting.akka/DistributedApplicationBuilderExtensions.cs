using System;
using System.Collections.Generic;
using System.Linq;
using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public static class DistributedApplicationBuilderExtensions
{
    public static IDistributedApplicationBuilder AddAkka(this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        string? systemName = null,
        Action<AkkaResourceBuilder>? akkaConfigure = null,
        Action<IResourceBuilder<AkkaResource>>? resourceConfigure = null)
    {
        systemName ??= name;
        var akkaResourceBuilder = new AkkaResourceBuilder(name, systemName, builder);
        akkaConfigure?.Invoke(akkaResourceBuilder);
        var resourceBuilder = akkaResourceBuilder.Build();
        resourceConfigure?.Invoke(resourceBuilder);
        return builder;
    }

    internal static ReferenceExpression GetSeedNodesOneliner(this IDistributedApplicationBuilder builder,
        string systemName)
    {
        var referenceExpressionBuilder = new ReferenceExpressionBuilder();
        var seedNodesExpressions = builder.GetSeedNodeAddresse(systemName);
        foreach (var referenceExpression in seedNodesExpressions)
        {
            referenceExpressionBuilder.AppendFormatted(referenceExpression);

            if (seedNodesExpressions.IndexOf(referenceExpression) != seedNodesExpressions.Length - 1)
            {
                referenceExpressionBuilder.AppendFormatted(",");
            }
        }

        return referenceExpressionBuilder.Build();
    }

    internal static ReferenceExpression[] GetSeedNodeAddresse(this IDistributedApplicationBuilder builder,
        string systemName)
    {
        var seedNodes = builder.GetSeedNodes(systemName);

        return seedNodes.Select(kvp =>
        {
            var resource = kvp.Key;
            var annotation = kvp.Value;

            var endpointReference = resource.GetEndpoint(annotation.EndpointName);
            return ReferenceExpression.Create($"akka.tcp://{systemName}@{endpointReference.Property(EndpointProperty.Host)}:{endpointReference.Property(EndpointProperty.Port)}");
        }).ToArray();
    }

    internal static IDictionary<IResourceWithEndpoints, AkkaSeedNodeAnnotation> GetSeedNodes(
        this IDistributedApplicationBuilder builder, string systemName)
    {
        return builder.Resources
            .OfType<IResourceWithEndpoints>()
            .Where(r =>
                r.Annotations
                    .OfType<AkkaClusterAnnotation>()
                    .FirstOrDefault()?.SystemName == systemName)
            .SelectMany(r =>
                r.Annotations
                    .OfType<AkkaSeedNodeAnnotation>()
                    .Select(a => (Resource: r, Annotation: a)))
            .ToDictionary(key => key.Resource, value => value.Annotation);
    }

    internal static List<IResourceWithEnvironment> GetNodes(this IDistributedApplicationBuilder builder,
        string systemName)
    {
        return builder.Resources
            .OfType<IResourceWithEnvironment>()
            .Where(r =>
                r.Annotations
                    .OfType<AkkaClusterAnnotation>()
                    .FirstOrDefault()?.SystemName == systemName)
            .Where(r => r.Annotations.OfType<AkkaNodeAnnotation>().Any())
            .Select(x => x)
            .ToList();
    }
}