using Akka.Core.Startup;
using Akka.Node.Two.Startup;
using Serilog;
using Servus.Core.Application.Startup;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateBootstrapLogger();

var runner = AppBuilder.Create(WebApplication.CreateBuilder(args), b => b.Build())
    .WithSetup<LoggingSetupContainer>()
    .WithSetup<HostApplicationBuilderSetupContainer>()
    .WithSetup<HostConfigurationSetupContainer>()
    .WithSetup<AkkaStartupContainer>();