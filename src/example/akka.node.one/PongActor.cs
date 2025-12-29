using akka.core;
using Servus.Akka.Diagnostics;

namespace akka.node.one;

public class PongActor : TracedMessageActor
{
    public PongActor(ILogger<PongActor> logger)
    {
        Receive<Pong>(ping =>
        {
            logger.LogDebug("PongActor: Pong received: {Name}", ping.GetType().Name);
            if (ping.Ack)
            {
                ReplyTraced(new Ping(true));
            }
        });
    }
}