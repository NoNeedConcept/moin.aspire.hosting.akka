using Microsoft.Extensions.Configuration;
using Servus.Core.Application.Startup;

namespace Akka.Core.Startup;

public class ConfigurationSetupContainer : IConfigurationSetupContainer
{
    public void SetupConfiguration(IConfigurationManager builder)
    {
        builder.AddEnvironmentVariables();
    }
}