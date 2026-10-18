using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.CommunityOps_Api>("api")
    .WithHttpHealthCheck("/health");

builder.AddViteApp("frontend", "../../frontend")
    .WithEnvironment("API_BASE_URL", api.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();
