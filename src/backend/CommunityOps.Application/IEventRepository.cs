using CommunityOps.Domain;

namespace CommunityOps.Application;

public interface IEventRepository
{
    Task<IReadOnlyList<CommunityEvent>> ListAsync(CancellationToken cancellationToken);
}
