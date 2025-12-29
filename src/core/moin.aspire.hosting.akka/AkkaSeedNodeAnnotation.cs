using System.Diagnostics;

namespace Aspire.Hosting.ApplicationModel;

[DebuggerDisplay("Type = {GetType().Name,nq}, Name = {EndpointName}")]
public class AkkaSeedNodeAnnotation : IResourceAnnotation
{
    public AkkaSeedNodeAnnotation(string endpointName = AkkaResourceBuilderExtensions.AkkaEndpointName)
    {
        EndpointName = endpointName;
    }

    public string EndpointName { get; }
}