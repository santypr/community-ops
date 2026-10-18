using CommunityOps.Application;
using CommunityOps.Domain;

namespace CommunityOps.Infrastructure;

public sealed class InMemoryEventRepository : IEventRepository
{
    private static readonly IReadOnlyList<CommunityEvent> Events = Array.AsReadOnly<CommunityEvent>(
    [
        new(Guid.Parse("e4a9953c-3cd9-428e-aeb3-f87eb2e87837"), "community-ops",
            "Encuentro de la comunidad · demo",
            new DateTimeOffset(2030, 1, 15, 18, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2030, 1, 15, 19, 0, 0, TimeSpan.Zero), "Online")
    ]);

    public Task<IReadOnlyList<CommunityEvent>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Events);
    }
}
