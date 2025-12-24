using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting.Akka;

public class AkkaClusterResource(string name, string actorSystemName) : Resource(name)
{
    public string ActorSystemName =>  actorSystemName; 
}