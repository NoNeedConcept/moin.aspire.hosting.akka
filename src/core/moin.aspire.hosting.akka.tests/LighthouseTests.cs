using Aspire.Hosting;

namespace moin.aspire.hosting.akka.tests;

public class LighthouseTests
{
    [Fact]
    public void WithLighthouse_AddsLighthouseResourceWithCorrectConfiguration()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var akka = new AkkaResourceBuilder("myakka", "mysystem", builder);

        // Act
        akka.WithLighthouse("mylighthouse", clusterPort: 5000, pbmPort: 9000);

        // Assert
        var lighthouse = builder.Resources.OfType<LighthouseResource>().FirstOrDefault(r => r.Name == "mylighthouse");
        Assert.NotNull(lighthouse);

        // Check Annotations
        Assert.Contains(lighthouse.Annotations, a => a is AkkaClusterAnnotation ca && ca.SystemName == "mysystem");
        Assert.Contains(lighthouse.Annotations, a => a is LighthouseAnnotation);
        Assert.Contains(lighthouse.Annotations, a => a is AkkaSeedNodeAnnotation);
        Assert.Contains(lighthouse.Annotations, a => a is AkkaNodeAnnotation ana && ana.Options?.SeedNodeEnvName == LighthouseResource.ClusterSeedsEnvName);

        // Check Endpoints
        var clusterEndpoint = lighthouse.Annotations.OfType<EndpointAnnotation>().FirstOrDefault(a => a.Name == "akka");
        Assert.NotNull(clusterEndpoint);
        Assert.Equal(5000, clusterEndpoint.Port);
        Assert.Equal(4053, clusterEndpoint.TargetPort);
        Assert.Equal("akka.tcp", clusterEndpoint.UriScheme);
        var pbmEndpoint = lighthouse.Annotations.OfType<EndpointAnnotation>().FirstOrDefault(a => a.Name == LighthouseResource.PbmEndpointName);
        Assert.NotNull(pbmEndpoint);
        Assert.Equal(9000, pbmEndpoint.Port);
        Assert.Equal(9011, pbmEndpoint.TargetPort);
    }

    [Fact]
    public void WithLighthouse_Replicas_AddsMultipleResources()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var akka = new AkkaResourceBuilder("myakka", "mysystem", builder);

        // Act
        akka.WithLighthouse(replicas: 3);

        // Assert
        var lighthouses = builder.Resources.OfType<LighthouseResource>().ToList();
        Assert.Equal(3, lighthouses.Count);
        Assert.Contains(lighthouses, r => r.Name == "lighthouse-0");
        Assert.Contains(lighthouses, r => r.Name == "lighthouse-1");
        Assert.Contains(lighthouses, r => r.Name == "lighthouse-2");
    }

    [Fact]
    public void WithLighthouse_SetsEnvironmentVariables()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var akka = new AkkaResourceBuilder("myakka", "mysystem", builder);

        // Act
        akka.WithLighthouse("mylighthouse", clusterPort: 5000, pbmPort: 9000);

        // Assert
        var lighthouse = builder.Resources.OfType<LighthouseResource>().First(r => r.Name == "mylighthouse");
        
        var config = new EnvironmentCallbackContext(new DistributedApplicationExecutionContext(DistributedApplicationOperation.Run));
        foreach (var callback in lighthouse.Annotations.OfType<EnvironmentCallbackAnnotation>())
        {
            callback.Callback(config);
        }

        Assert.Equal("mysystem", config.EnvironmentVariables[LighthouseResource.ActorSystemNameEnvName]);
        Assert.True(config.EnvironmentVariables.ContainsKey(LighthouseResource.ClusterIpEnvName));
    }
}
