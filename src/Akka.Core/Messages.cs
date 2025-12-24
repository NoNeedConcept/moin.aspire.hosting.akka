namespace Akka.Core;

public record Ping(bool Ack = false);

public record Pong(bool Ack = false);