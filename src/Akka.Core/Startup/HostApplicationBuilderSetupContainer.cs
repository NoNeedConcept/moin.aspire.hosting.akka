using Microsoft.Extensions.Hosting;
using Servus.Core.Application.Startup;

namespace Akka.Core.Startup;

public class HostApplicationBuilderSetupContainer : IHostApplicationBuilderSetupContainer
{
    public void ConfigureHostApplicationBuilder(IHostApplicationBuilder builder)
    {
        builder.AddServiceDefaults();
    }
}