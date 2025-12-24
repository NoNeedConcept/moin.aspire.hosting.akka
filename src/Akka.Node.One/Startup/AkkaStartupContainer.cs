using Akka.Cluster.Hosting;
using Akka.Core;
using Akka.Hosting;
using Akka.Cluster.Hosting.Aspire;
using Akka.Remote.Hosting.Aspire;
using moin.akka.endpoint;
using Servus.Akka.Startup;

namespace Akka.Node.One.Startup;

public class AkkaStartupContainer : ActorSystemSetupContainer
{
    protected override string GetActorSystemName()
    {
        return "testing";
    }

    protected override void BuildSystem(AkkaConfigurationBuilder builder, IServiceProvider serviceProvider)
    {
        builder
            .WithRemoting(serviceProvider)
            .WithClustering(serviceProvider)
            .WithActorSystemLivenessCheck()
            .WithAkkaClusterReadinessCheck()
            .AddService<PongActor, PongEndpoint.Pong>()
            .AddClient<PingEndpoint.Ping>()
            .AddStartup(async (system, registry) => { await Task.CompletedTask; });
    }
}