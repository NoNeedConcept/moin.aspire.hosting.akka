using Microsoft.Extensions.Configuration;
using Servus.Core.Application.Startup;

namespace akka.core.Startup;

public class ConfigurationSetupContainer : IConfigurationSetupContainer
{
    public void SetupConfiguration(IConfigurationManager builder)
    {
        builder.AddEnvironmentVariables();
    }
}