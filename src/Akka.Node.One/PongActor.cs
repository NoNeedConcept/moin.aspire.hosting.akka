using Akka.Actor;
using Akka.Core;

namespace Akka.Node.One;

public class PongActor : ReceiveActor
{
    public PongActor()
    {
        var random = new Random(1337);
        Receive<Ping>(ping =>
        {
            if (ping.Ack && random.NextInt64(0, 50) % 1 == 0)
            {
                Sender.Tell(new Pong(Convert.ToBoolean(random.GetItems(new[] { 0, 1 }, 2))));
            }
        });
    }
}