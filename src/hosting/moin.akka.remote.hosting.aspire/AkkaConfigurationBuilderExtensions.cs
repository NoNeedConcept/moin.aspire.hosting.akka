using Akka.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akka.Remote.Hosting;

public static class AkkaConfigurationBuilderExtensions
{
    public static AkkaConfigurationBuilder WithRemoting(this AkkaConfigurationBuilder builder,
        IServiceProvider provider, string sectionName = "akka:remote:dot-netty:tcp", Action<RemoteOptions>? configure = null)
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var remoteSection = configuration.GetSection(sectionName);
        var remoteOptions = new RemoteOptions
        {
            Port = remoteSection.GetSection("port").Get<int?>(),
            PublicHostName =  remoteSection.GetSection("public-hostname").Get<string>(),
        };
        configure?.Invoke(remoteOptions);
        return builder.WithRemoting(remoteOptions);
    }
}