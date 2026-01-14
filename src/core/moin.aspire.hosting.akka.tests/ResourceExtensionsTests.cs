using Aspire.Hosting;

namespace moin.aspire.hosting.akka.tests;

public class ResourceExtensionsTests
{
    [Fact]
    public void GetSeedNodeEnvNameOptions_ReturnsDefault_WhenNoAnnotation()
    {
        // Arrange
        var resource = new TestResource("test");
        var defaultOptions = new SeedNodeEnvNameOptions();

        // Act
        var result = resource.GetSeedNodeEnvNameOptions(defaultOptions);

        // Assert
        Assert.Same(defaultOptions, result);
    }

    [Fact]
    public void GetSeedNodeEnvNameOptions_ReturnsAnnotationOptions_WhenPresent()
    {
        // Arrange
        var resource = new TestResource("test");
        var customOptions = new SeedNodeEnvNameOptions { SeedNodeEnvName = "CUSTOM" };
        resource.Annotations.Add(new AkkaNodeAnnotation(customOptions));
        var defaultOptions = new SeedNodeEnvNameOptions();

        // Act
        var result = resource.GetSeedNodeEnvNameOptions(defaultOptions);

        // Assert
        Assert.Same(customOptions, result);
    }

    private class TestResource(string name) : Resource(name)
    {
    }
}
