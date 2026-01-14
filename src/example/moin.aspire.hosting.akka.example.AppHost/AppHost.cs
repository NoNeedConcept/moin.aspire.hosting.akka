var builder = DistributedApplication.CreateBuilder(args);

var nodeOne = builder
    .AddDockerfile("AkkaNodeOne", "../../", "example/akka.node.one/Dockerfile")
    .WithContainerName("akka.node.one")
    .WithImageRegistry("localhost")
    .WithImageTag("dev")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithOtlpExporter()
    .WithHttpEndpoint(targetPort: 8080)
    .WithHttpHealthCheck(path: "/health")
    .WithHttpHealthCheck(path: "/alive")
    .WithHttpHealthCheck(path: "/started");

var nodeTwo = builder
    .AddDockerfile("AkkaNodeTwo", "../../", "example/akka.node.two/Dockerfile")
    .WithContainerName("akka.node.two")
    .WithImageRegistry("localhost")
    .WithImageTag("dev")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithOtlpExporter()
    .WithHttpEndpoint(targetPort: 8080)
    .WithHttpHealthCheck(path: "/health")
    .WithHttpHealthCheck(path: "/alive")
    .WithHttpHealthCheck(path: "/started");

builder.AddAkka("akka", "testing", akkaConfigure: akkaBuilder =>
{
    akkaBuilder
        .WithLighthouse(3)
        .WithNode(targetPort: 8000, resource: nodeOne, endpointName: "akka")
        .WithNode(targetPort: 8001, resource: nodeTwo, endpointName: "akka");
});

await builder.Build().RunAsync();