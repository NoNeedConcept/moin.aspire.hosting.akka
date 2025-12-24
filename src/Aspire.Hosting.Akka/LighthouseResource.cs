using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting.Akka;

public class LighthouseResource(string name) : ContainerResource(name)
{
    internal const string PbmEndpointName = "pbm";
    internal const string TcpEndpointName = "tcp";

    internal const string ActorSystemNameEnvName = "ACTORSYSTEM";
    internal const string ClusterPortEnvName = "CLUSTER_PORT";
    internal const string ClusterIpEnvName = "CLUSTER_IP";
    internal const string ClusterSeedsEnvName = "CLUSTER_SEEDS";
    
    internal EndpointReference TcpEndpoint => field ??= new EndpointReference(this, TcpEndpointName);
    internal EndpointReference PbmEndpoint => field ??= new EndpointReference(this, PbmEndpointName);
}