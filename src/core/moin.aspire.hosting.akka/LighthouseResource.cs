namespace Aspire.Hosting.ApplicationModel;

public class LighthouseResource(string name) : ContainerResource(name)
{
    internal const string PbmEndpointName = "pbm";

    internal const string ActorSystemNameEnvName = "ACTORSYSTEM";
    internal const string ClusterPortEnvName = "CLUSTER_PORT";
    internal const string ClusterIpEnvName = "CLUSTER_IP";
    internal const string ClusterSeedsEnvName = "CLUSTER_SEEDS";
}