var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.EventPlatform_ApiService>("apiservice");

builder.AddProject<Projects.EventPlatform_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
