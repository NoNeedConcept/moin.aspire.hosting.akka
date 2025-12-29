using Akka.Cluster.Hosting;
using akka.core;
using Akka.Hosting;
using Akka.Remote.Hosting;
using moin.akka.endpoint;
using Servus.Akka.Startup;

namespace akka.node.two.Startup;

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
                options.ClusterReadyCheck = true;
            })
            .WithActorSystemLivenessCheck(tags: ["akka", "live"])
            .AddService<PingActor, PingEndpoint.Ping>()
            .AddClient<PingEndpoint.Ping>()
            .AddClient<PongEndpoint.Pong>()
            .AddStartup(async (_, registry) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                registry.GetClient<PongEndpoint.Pong>().Tell(new Pong(true), registry.Get<PingEndpoint.Ping>());
            });
    }
}