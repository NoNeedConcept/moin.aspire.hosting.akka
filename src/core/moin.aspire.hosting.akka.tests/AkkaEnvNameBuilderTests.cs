using Aspire.Hosting;

namespace moin.aspire.hosting.akka.tests;

public class AkkaEnvNameBuilderTests
{
    [Fact]
    public void DefaultConstructor_SetsDefaultValues()
    {
        // Act
        var builder = new AkkaEnvNameBuilder();

        // Assert
        Assert.NotNull(builder.SeedNodeEnvNameOptions);
        Assert.Equal(AkkaDefaultEnvName.HostnameEnvName, builder.HostnameEnvName);
        Assert.Equal(AkkaDefaultEnvName.PortEnvName, builder.PortEnvName);
    }

    [Fact]
    public void WithHostnameEnvName_UpdatesHostnameEnvName()
    {
        // Arrange
        var builder = new AkkaEnvNameBuilder();
        var customName = "CUSTOM_HOSTNAME";

        // Act
        builder.WithHostnameEnvName(customName);

        // Assert
        Assert.Equal(customName, builder.HostnameEnvName);
    }

    [Fact]
    public void WithPortEnvName_UpdatesPortEnvName()
    {
        // Arrange
        var builder = new AkkaEnvNameBuilder();
        var customName = "CUSTOM_PORT";

        // Act
        builder.WithPortEnvName(customName);

        // Assert
        Assert.Equal(customName, builder.PortEnvName);
    }

    [Fact]
    public void WithSeedNodeEnvName_UpdatesSeedNodeEnvNameOptions()
    {
        // Arrange
        var builder = new AkkaEnvNameBuilder();
        var customSeedNodeEnvName = "CUSTOM_SEEDS";

        // Act
        builder.WithSeedNodeEnvName(options =>
        {
            options.SeedNodeEnvName = customSeedNodeEnvName;
            options.Mode = EnvValueMode.SingleValue;
        });

        // Assert
        Assert.Equal(customSeedNodeEnvName, builder.SeedNodeEnvNameOptions.SeedNodeEnvName);
        Assert.Equal(EnvValueMode.SingleValue, builder.SeedNodeEnvNameOptions.Mode);
    }
}
