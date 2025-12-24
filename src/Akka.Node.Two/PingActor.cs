using Akka.Actor;
using Akka.Core;

namespace Akka.Node.Two;

public class PingActor : ReceiveActor
{
    public PingActor()
    {
        var random = new Random(1337);
        Receive<Pong>(pong =>
        {
            if (pong.Ack && random.NextInt64(0, 50) % 1 == 0)
            {
                Sender.Tell(new Pong(Convert.ToBoolean(random.GetItems(new[] { 0, 1 }, 2))));
            }
        });
    }
}