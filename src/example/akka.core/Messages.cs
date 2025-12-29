using Servus.Core.Diagnostics;

namespace akka.core;

public record Ping(bool Ack = false) : TracedMessage;

public record Pong(bool Ack = false) : TracedMessage;

public record TracedMessage : IWithTracing
{
    public string? TraceId { get; set; }
    public string? SpanId { get; set; }
}