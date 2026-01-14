using Aspire.Hosting.ApplicationModel;
using moin.aspire.hosting.akka;

namespace Aspire.Hosting;

public interface IAkkaResourceBuilder
{
    IDistributedApplicationBuilder DistributedApplicationBuilder { get; }
    string SystemName { get; }
    SeedNodeEnvNameOptions SeedNodeEnvNameOptionDefault { get; }
    string HostnameEnvNameDefault { get; }
    string PortEnvNameDefault { get; }
    IResourceBuilder<T> AddResource<T>(T instance) where T : IResource;
    AkkaEnvNameBuilder CreateEnvNameBuilder();
}