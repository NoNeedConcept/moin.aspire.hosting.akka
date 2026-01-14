using Akka.Cluster.Hosting;
using akka.core;
using Akka.Hosting;
using Akka.Remote.Hosting;
using moin.akka.endpoint;
using Servus.Akka.Startup;

namespace akka.node.one.Startup;

public class AkkaStartupContainer : ActorSystemSetupContainer
{
    protected override string GetActorSystemName()
    {
        return "testing";
    }

    protected override void BuildSystem(AkkaConfigurationBuilder builder, IServiceProvider serviceProvider)
    {
        builder
            .WithRemoting(serviceProvider, configure: options => { options.HostName = "0.0.0.0"; })
            .WithClustering(serviceProvider, configure: options =>
            {
                options.Roles = ["ping"];
            })
            .WithActorSystemLivenessCheck(tags: ["akka", "live"])
            .WithAkkaClusterReadinessCheck()
            .AddService<PongActor, PongEndpoint.Pong>()
            .AddClient<PongEndpoint.Pong>()
            .AddClient<PingEndpoint.Ping>()
            .AddStartup(async (_, registry) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                registry.GetClient<PingEndpoint.Ping>().Tell(new Ping(true), registry.Get<PongEndpoint.Pong>());
            });
    }
}