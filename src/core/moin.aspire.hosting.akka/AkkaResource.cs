namespace Aspire.Hosting.ApplicationModel;

public class AkkaResource(
    string name,
    string systemName,
    IResourceWithEndpoints[] seedNodes,
    IResourceWithEnvironment[] nodes) : Resource(name), IResourceWithoutLifetime
{
    public string SystemName => systemName;
    public IResourceWithEndpoints[] SeedNodes => seedNodes;
    public IResourceWithEnvironment[] Nodes => nodes;
}