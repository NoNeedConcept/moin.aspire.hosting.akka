namespace Aspire.Hosting.ApplicationModel;

public class AkkaClusterAnnotation : IResourceAnnotation
{
    public AkkaClusterAnnotation(string systemName)
    {
        SystemName = systemName;
    }
    
    public string SystemName { get; }
}