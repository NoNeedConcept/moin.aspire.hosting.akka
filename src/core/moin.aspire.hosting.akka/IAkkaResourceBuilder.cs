using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public interface IAkkaResourceBuilder
{
    IDistributedApplicationBuilder DistributedApplicationBuilder { get; }
    string SystemName { get; }
    IResourceBuilder<T> AddResource<T>(T instance) where T : IResource;
}