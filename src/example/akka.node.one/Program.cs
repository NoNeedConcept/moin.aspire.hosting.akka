using akka.core.Startup;
using akka.node.one.Startup;
using Serilog;
using Servus.Core.Application.Startup;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateBootstrapLogger();

var runner = AppBuilder.Create()
    .WithSetup<ConfigurationSetupContainer>()
    .WithSetup<LoggingSetupContainer>()
    .WithSetup<HostApplicationBuilderSetupContainer>()
    .WithSetup<HostConfigurationSetupContainer>()
    .WithSetup<AkkaStartupContainer>();

await runner.Build().RunAsync();