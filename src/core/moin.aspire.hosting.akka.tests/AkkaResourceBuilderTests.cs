using Aspire.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace moin.aspire.hosting.akka.tests;

public class AkkaResourceBuilderTests
{
    [Fact]
    public void AddAkka_AddsAkkaResourceToBuilder()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        builder.AddAkka("myakka", "mysystem");

        // Assert
        var akkaResource = builder.Resources.OfType<AkkaResource>().FirstOrDefault();
        Assert.NotNull(akkaResource);
        Assert.Equal("myakka", akkaResource.Name);
        Assert.Equal("mysystem", akkaResource.SystemName);
    }

    [Fact]
    public void WithNode_AddsAnnotationsAndEndpoint()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var node = builder.AddContainer("node1", "image1");
        var akka = new AkkaResourceBuilder("akka", "system", builder);

        // Act
        akka.WithNode(node, targetPort: 8080);

        // Assert
        Assert.Contains(node.Resource.Annotations, a => a is AkkaClusterAnnotation ca && ca.SystemName == "system");
        Assert.Contains(node.Resource.Annotations, a => a is AkkaNodeAnnotation);
        Assert.Contains(node.Resource.Annotations, a => a is EndpointAnnotation ea && ea.Name == "akka" && ea.TargetPort == 8080);
    }

    [Fact]
    public void WithSeedNode_AddsSeedNodeAnnotation()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var node = builder.AddContainer("seed1", "image1");
        var akka = new AkkaResourceBuilder("akka", "system", builder);

        // Act
        akka.WithSeedNode(node, targetPort: 8080);

        // Assert
        Assert.Contains(node.Resource.Annotations, a => a is AkkaSeedNodeAnnotation);
    }

    [Fact]
    public void Build_ConfiguresEnvironmentVariables_ArrayMode()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var seed = builder.AddContainer("seed1", "image1");
        
        var node = builder.AddContainer("node1", "image1");
        
        builder.AddAkka("akka", "system", akka =>
        {
            akka.WithCustomEnvNames(env => env.WithSeedNodeEnvName(o => o.Mode = EnvValueMode.Array));
            akka.WithSeedNode(seed, targetPort: 8080);
            akka.WithNode(node, targetPort: 8081);
        });

        // Act
        // AkkaResourceBuilder.Build is called inside AddAkka

        // Assert
        var config = new EnvironmentCallbackContext(new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run));
        foreach (var callback in node.Resource.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            callback.Callback(config);
        }

        Assert.True(config.EnvironmentVariables.ContainsKey(AkkaDefaultEnvName.SeedNodeEnvName + "__0"));
        var val = config.EnvironmentVariables[AkkaDefaultEnvName.SeedNodeEnvName + "__0"];
        Assert.NotNull(val);
    }

    [Fact]
    public void Build_ConfiguresEnvironmentVariables_SingleValueMode()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var seed = builder.AddContainer("seed1", "image1");
        
        var node = builder.AddContainer("node1", "image1");
        
        builder.AddAkka("akka", "system", akka =>
        {
            akka.WithCustomEnvNames(env => env.WithSeedNodeEnvName(o => o.Mode = EnvValueMode.SingleValue));
            akka.WithSeedNode(seed, targetPort: 8080);
            akka.WithNode(node, targetPort: 8081);
        });

        // Act
        // AkkaResourceBuilder.Build is called inside AddAkka

        // Assert
        var config = new EnvironmentCallbackContext(new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run));
        foreach (var callback in node.Resource.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            callback.Callback(config);
        }

        Assert.True(config.EnvironmentVariables.ContainsKey(AkkaDefaultEnvName.SeedNodeEnvName));
        var val = config.EnvironmentVariables[AkkaDefaultEnvName.SeedNodeEnvName];
        Assert.NotNull(val);
    }

    [Fact]
    public void Build_ConfiguresEnvironmentVariables_CustomNodeOptions()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var seed = builder.AddContainer("seed1", "image1");
        
        var node = builder.AddContainer("node1", "image1");
        
        builder.AddAkka("akka", "system", akka =>
        {
            akka.WithSeedNode(seed, targetPort: 8080);
            akka.WithNode(node, targetPort: 8081, configure: env => env.WithSeedNodeEnvName(o => 
            {
                o.SeedNodeEnvName = "CUSTOM_SEEDS";
                o.Mode = EnvValueMode.SingleValue;
            }));
        });

        // Act
        // AkkaResourceBuilder.Build is called inside AddAkka

        // Assert
        var config = new EnvironmentCallbackContext(new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run));
        foreach (var callback in node.Resource.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            callback.Callback(config);
        }

        Assert.True(config.EnvironmentVariables.ContainsKey("CUSTOM_SEEDS"));
    }

    [Fact]
    public void AddAkka_SetsUpHealthCheck()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();

        // Act
        builder.AddAkka("myakka", "mysystem");

        // Assert
        var healthCheckRegistrations = builder.Services.BuildServiceProvider()
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<HealthCheckServiceOptions>>().Value.Registrations;
        
        Assert.Contains(healthCheckRegistrations, r => r.Name == "akka-cluster-mysystem");
    }
}
