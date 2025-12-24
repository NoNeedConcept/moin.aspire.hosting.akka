var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Akka_Node_One>("AkkaNodeOne")
    .AsSeedNode();

builder.AddProject<Projects.Akka_Node_Two>("AkkaNodeTwo")
    .AsAkkaNode();

builder.AddAkkaClusterWithLighthouse("testing");

builder.FinalizeAkkaCluster();

builder.Build().Run();