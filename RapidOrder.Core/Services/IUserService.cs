using RapidOrder.Core.Entities;

namespace RapidOrder.Core.Services;

public interface IUserService
{
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task AssignMissionAsync(long userId, Mission mission, CancellationToken cancellationToken = default);
}
