using Akka.Cluster.Hosting;
using Akka.Core;
using Akka.Hosting;
using moin.akka.endpoint;
using Servus.Akka.Startup;

namespace Akka.Node.Two.Startup;

public class AkkaStartupContainer : ActorSystemSetupContainer
{
    protected override string GetActorSystemName()
    {
        return "testing";
    }

    protected override void BuildSystem(AkkaConfigurationBuilder builder, IServiceProvider serviceProvider)
    {
        builder
            .WithActorSystemLivenessCheck()
            .WithAkkaClusterReadinessCheck()
            .AddService<PingActor, PingEndpoint.Ping>()
            .AddClient<PongEndpoint.Pong>()
            .AddStartup(async (system, registry) =>
            {
                await Task.CompletedTask;
            });
    }
}