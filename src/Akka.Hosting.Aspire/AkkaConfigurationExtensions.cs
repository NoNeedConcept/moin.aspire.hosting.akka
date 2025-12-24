namespace Akka.Hosting.Aspire;

public static class AkkaConfigurationExtensions
{
    public static AkkaConfigurationBuilder Test(this AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        return builder;
    }
}