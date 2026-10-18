using Azure.Identity;
using CommunityOps.Application;
using CommunityOps.Infrastructure;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<ListEvents>();

builder.Services.AddSingleton(services =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var endpoint = configuration["Cosmos:Endpoint"];
    if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != "https")
        throw new InvalidOperationException("Configura Cosmos:Endpoint con una URL HTTPS.");

    return CosmosEventRepository.CreateClient(endpoint!, new DefaultAzureCredential());
});
builder.Services.AddSingleton<IEventRepository>(services =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var environment = services.GetRequiredService<IHostEnvironment>();
    var eventStore = configuration["Events:Store"] ?? "Cosmos";
    if (eventStore == "InMemory" && environment.IsDevelopment())
        return new InMemoryEventRepository();
    if (eventStore != "Cosmos")
        throw new InvalidOperationException("Events:Store debe ser Cosmos; InMemory solo está permitido en Development.");

    var database = configuration["Cosmos:Database"] ?? "community-ops";
    var container = configuration["Cosmos:Container"] ?? "events";
    return new CosmosEventRepository(services.GetRequiredService<CosmosClient>().GetContainer(database, container));
});
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    if (origins.Length > 0)
        policy.WithOrigins(origins).WithMethods("GET").AllowAnyHeader();
}));

var app = builder.Build();
_ = app.Services.GetRequiredService<IEventRepository>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors();
app.MapHealthChecks("/health");
app.MapGet("/api/events", async (ListEvents useCase, CancellationToken cancellationToken) =>
    Results.Ok(await useCase.ExecuteAsync(cancellationToken)));
app.Run();

public partial class Program;
