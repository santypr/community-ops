using CommunityOps.Application;
using CommunityOps.Domain;
using CommunityOps.Infrastructure;

namespace CommunityOps.Tests;

public sealed class EventTests
{
    [Fact]
    public void EventNormalizesTextAndUtcDates()
    {
        var start = new DateTimeOffset(2030, 1, 15, 18, 0, 0, TimeSpan.FromHours(2));
        var item = new CommunityEvent(Guid.NewGuid(), " community ", " Event ",
            start, start.AddHours(1), " Online ");

        Assert.Equal("community", item.CommunityId);
        Assert.Equal("Event", item.Title);
        Assert.Equal("Online", item.Location);
        Assert.Equal(TimeSpan.Zero, item.StartsAt.Offset);
        Assert.Equal(start.UtcDateTime, item.StartsAt.UtcDateTime);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void EventRejectsInvalidDuration(int hours)
    {
        var start = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentException>(() => new CommunityEvent(
            Guid.NewGuid(), "community", "Event", start, start.AddHours(hours), "Online"));
    }

    [Theory]
    [InlineData("", "Event", "Online")]
    [InlineData("community", " ", "Online")]
    [InlineData("community", "Event", "")]
    public void EventRejectsMissingText(string community, string title, string location)
    {
        var start = DateTimeOffset.UtcNow;
        Assert.ThrowsAny<ArgumentException>(() => new CommunityEvent(
            Guid.NewGuid(), community, title, start, start.AddHours(1), location));
    }

    [Fact]
    public void EventRejectsEmptyId()
    {
        var start = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentException>(() => new CommunityEvent(
            Guid.Empty, "community", "Event", start, start.AddHours(1), "Online"));
    }

    [Fact]
    public async Task UseCaseReturnsRepositoryEvents()
    {
        var result = await new ListEvents(new InMemoryEventRepository()).ExecuteAsync(CancellationToken.None);
        var item = Assert.Single(result);
        Assert.Equal("community-ops", item.CommunityId);
    }

    [Fact]
    public async Task UseCaseSupportsEmptyCatalog()
    {
        var result = await new ListEvents(new EmptyRepository()).ExecuteAsync(CancellationToken.None);
        Assert.Empty(result);
    }

    [Fact]
    public async Task RepositoryHonorsCancellation()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => new ListEvents(new InMemoryEventRepository()).ExecuteAsync(source.Token));
    }

    private sealed class EmptyRepository : IEventRepository
    {
        public Task<IReadOnlyList<CommunityEvent>> ListAsync(CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<CommunityEvent>>([]);
    }
}
