namespace CommunityOps.Application;

public sealed record EventResponse(
    Guid Id, string CommunityId, string Title,
    DateTimeOffset StartsAt, DateTimeOffset EndsAt, string Location);

public sealed class ListEvents(IEventRepository repository)
{
    public async Task<IReadOnlyList<EventResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var events = await repository.ListAsync(cancellationToken);
        return events.Select(item => new EventResponse(
            item.Id, item.CommunityId, item.Title,
            item.StartsAt, item.EndsAt, item.Location)).ToArray();
    }
}
