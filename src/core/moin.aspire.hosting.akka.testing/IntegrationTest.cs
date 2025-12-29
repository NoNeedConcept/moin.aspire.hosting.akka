using Microsoft.Extensions.Logging;

namespace moin.aspire.hosting.akka.testing;

[Trait("Category", "Integration")]
public class IntegrationTest
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(180);

    // Instructions:
    // 1. Add a project reference to the target AppHost project, e.g.:
    //
    //    <ItemGroup>
    //        <ProjectReference Include="../MyAspireApp.AppHost/MyAspireApp.AppHost.csproj" />
    //    </ItemGroup>
    //
    // 2. Uncomment the following example test and update 'Projects.MyAspireApp_AppHost' to match your AppHost project:
    //
    [Fact]
    public async Task GetNodesStartedReturnsOkStatusCode()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.moin_aspire_hosting_akka_example_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
    
        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
    
        // Act
        using var nodeOneClient = app.CreateHttpClient("AkkaNodeOne");
        using var nodeTwoClient = app.CreateHttpClient("AkkaNodeTwo");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("AkkaNodeOne", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("AkkaNodeTwo", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        using var nodeOneResponse = await nodeOneClient.GetAsync("/started", cancellationToken);
        using var nodeTwoResponse = await nodeTwoClient.GetAsync("/started", cancellationToken);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, nodeOneResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, nodeTwoResponse.StatusCode);
    }

}