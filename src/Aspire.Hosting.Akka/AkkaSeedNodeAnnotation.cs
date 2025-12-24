using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting.Akka;

public class AkkaSeedNodeAnnotation : IResourceAnnotation
{
    public AkkaSeedNodeAnnotation(string endpointName = "akka")
    {
        EndpointName = endpointName;
    }

    public string EndpointName { get; }
}