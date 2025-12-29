using moin.akka.endpoint;

namespace akka.core;

[RoleDefinition("pong")]
public record PongEndpoint
{
    [EndpointDefinition("pong", EndpointType.Singleton)]
    public record Pong : IEndpointDefinition;
}

[RoleDefinition("ping")]
public record PingEndpoint
{
    [EndpointDefinition("ping", EndpointType.Singleton)]
    public record Ping : IEndpointDefinition;
}