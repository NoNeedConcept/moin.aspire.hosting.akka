using akka.core;
using Servus.Akka.Diagnostics;

namespace akka.node.two;

public class PingActor : TracedMessageActor
{
    public PingActor(ILogger<PingActor> logger)
    {
        Receive<Ping>(pong =>
        {
            logger.LogDebug("PingActor: Ping received: {Name}", pong.GetType().Name);
            if (pong.Ack)
            {
                ReplyTraced(new Pong(true));
            }
        });
    }
}