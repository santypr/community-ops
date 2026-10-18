using System.Net;
using System.Net.Http.Json;
using CommunityOps.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommunityOps.Tests;

public sealed class ApiTests
{
    [Fact]
    public async Task EventsReturnDocumentedJsonContract()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/api/events");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var events = await response.Content.ReadFromJsonAsync<EventResponse[]>();
        Assert.NotNull(events);
        Assert.Single(events);
        Assert.NotEqual(Guid.Empty, events[0].Id);
    }

    [Fact]
    public async Task HealthEndpointResponds()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        Assert.Equal("Healthy", await client.GetStringAsync("/health"));
    }

    [Fact]
    public async Task UnknownRouteReturnsProblemDetails()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/missing");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData("https://community.example", true)]
    [InlineData("https://untrusted.example", false)]
    public async Task CorsOnlyAllowsConfiguredOrigin(string origin, bool allowed)
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/events");
        request.Headers.Add("Origin", origin);
        using var response = await client.SendAsync(request);
        Assert.Equal(allowed, response.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task ProductionRejectsInMemoryStore()
    {
        await using var factory = CreateFactory("Production");
        var error = Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        Assert.Contains("InMemory", error.Message);
    }

    [Fact]
    public async Task EmptyCatalogReturnsEmptyArray()
    {
        await using var factory = CreateFactory().WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.Replace(ServiceDescriptor.Singleton<IEventRepository>(new EmptyRepository()))));
        using var client = factory.CreateClient();
        var events = await client.GetFromJsonAsync<EventResponse[]>("/api/events");
        Assert.NotNull(events);
        Assert.Empty(events);
    }

    [Fact]
    public async Task PersistenceFailureReturnsProblemDetailsWithoutInternalMessage()
    {
        await using var factory = CreateFactory("Production").WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
                services.Replace(ServiceDescriptor.Singleton<IEventRepository>(new FailingRepository()))));
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/api/events");
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.DoesNotContain("internal-database-detail", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task WritingEventsIsNotSupported()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/events", new { title = "Event" });
        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    private sealed class EmptyRepository : IEventRepository
    {
        public Task<IReadOnlyList<CommunityOps.Domain.CommunityEvent>> ListAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<CommunityOps.Domain.CommunityEvent>>([]);
    }

    private sealed class FailingRepository : IEventRepository
    {
        public Task<IReadOnlyList<CommunityOps.Domain.CommunityEvent>> ListAsync(CancellationToken cancellationToken) =>
            throw new InvalidOperationException("internal-database-detail");
    }

    private static WebApplicationFactory<Program> CreateFactory(string environment = "Development") =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Events:Store"] = "InMemory",
                    ["Cors:AllowedOrigins:0"] = "https://community.example"
                }));
        });
}
