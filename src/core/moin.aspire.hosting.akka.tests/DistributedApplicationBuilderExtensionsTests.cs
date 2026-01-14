using Aspire.Hosting;

namespace moin.aspire.hosting.akka.tests;

public class DistributedApplicationBuilderExtensionsTests
{
    [Fact]
    public void GetNodes_ReturnsCorrectNodes()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var akka = new AkkaResourceBuilder("myakka", "mysystem", builder);
        
        var node1 = builder.AddContainer("node1", "image");
        var node2 = builder.AddContainer("node2", "image");
        var otherNode = builder.AddContainer("other", "image");

        akka.WithNode(node1, 4053);
        akka.WithNode(node2, 4053);

        // Act
        var nodes = builder.GetNodes("mysystem");

        // Assert
        Assert.Equal(2, nodes.Count);
        Assert.Contains(node1.Resource, nodes);
        Assert.Contains(node2.Resource, nodes);
        Assert.DoesNotContain(otherNode.Resource, nodes);
    }

    [Fact]
    public void GetSeedNodes_ReturnsCorrectSeedNodes()
    {
        // Arrange
        var builder = DistributedApplication.CreateBuilder();
        var akka = new AkkaResourceBuilder("myakka", "mysystem", builder);
        
        var seed1 = builder.AddContainer("seed1", "image");
        var node1 = builder.AddContainer("node1", "image");

        akka.WithSeedNode(seed1, 4053);
        akka.WithNode(node1, 4053);

        // Act
        var seedNodes = builder.GetSeedNodes("mysystem");

        // Assert
        Assert.Single(seedNodes);
        Assert.Equal(seed1.Resource, seedNodes.Keys.First());
    }
}
