using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Servus.Core.Application.Startup;

namespace akka.core.Startup;

public class HostConfigurationSetupContainer : ApplicationSetupContainer<WebApplication>
{
    protected override void SetupApplication(WebApplication app)
    {
        app.MapDefaultEndpoints();
    }
}