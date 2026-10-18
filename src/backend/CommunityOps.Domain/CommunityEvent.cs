namespace CommunityOps.Domain;

public sealed record CommunityEvent
{
    public Guid Id { get; }
    public string CommunityId { get; }
    public string Title { get; }
    public DateTimeOffset StartsAt { get; }
    public DateTimeOffset EndsAt { get; }
    public string Location { get; }

    public CommunityEvent(Guid id, string communityId, string title,
        DateTimeOffset startsAt, DateTimeOffset endsAt, string location)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("El identificador es obligatorio.", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(communityId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(location);
        if (endsAt <= startsAt)
            throw new ArgumentException("El evento debe terminar después de comenzar.", nameof(endsAt));

        Id = id;
        CommunityId = communityId.Trim();
        Title = title.Trim();
        StartsAt = startsAt.ToUniversalTime();
        EndsAt = endsAt.ToUniversalTime();
        Location = location.Trim();
    }
}
