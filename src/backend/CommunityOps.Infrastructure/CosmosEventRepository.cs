using System.Text.Json;
using CommunityOps.Application;
using CommunityOps.Domain;
using Microsoft.Azure.Cosmos;

namespace CommunityOps.Infrastructure;

public sealed class CosmosEventRepository(Container container) : IEventRepository
{
    public async Task<IReadOnlyList<CommunityEvent>> ListAsync(CancellationToken cancellationToken)
    {
        using var iterator = container.GetItemQueryIterator<EventDocument>(
            new QueryDefinition("SELECT TOP 50 * FROM c ORDER BY c.startsAt"),
            requestOptions: new QueryRequestOptions { MaxItemCount = 50 });

        var events = new List<CommunityEvent>();
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            events.AddRange(page.Select(item => new CommunityEvent(
                item.Id, item.CommunityId, item.Title, item.StartsAt, item.EndsAt, item.Location)));
        }
        return events;
    }

    private sealed record EventDocument(
        Guid Id, string CommunityId, string Title,
        DateTimeOffset StartsAt, DateTimeOffset EndsAt, string Location);

    public static CosmosClient CreateClient(string endpoint, Azure.Core.TokenCredential credential) =>
        new(endpoint, credential, new CosmosClientOptions
        {
            UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web),
            ConnectionMode = ConnectionMode.Gateway
        });
}
