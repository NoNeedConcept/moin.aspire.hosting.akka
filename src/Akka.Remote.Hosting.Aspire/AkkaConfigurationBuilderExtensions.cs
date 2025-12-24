using Akka.Hosting;
using Akka.Hosting.Aspire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Akka.Remote.Hosting.Aspire;

public static class AkkaConfigurationBuilderExtensions
{
    public static AkkaConfigurationBuilder WithRemoting(this AkkaConfigurationBuilder builder, IServiceProvider provider, string? sectionName = null, Action<RemoteOptions>? configure = null)
    {
        sectionName ??= AkkaConfigurationStrings.GetAkkaRemoteString();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var remoteOptions = configuration.GetSection(sectionName).Get<RemoteOptions>()!;
        configure?.Invoke(remoteOptions);
        return builder.WithRemoting(remoteOptions);
    }
}