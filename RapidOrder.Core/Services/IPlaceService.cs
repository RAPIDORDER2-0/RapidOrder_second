using RapidOrder.Core.Entities;

namespace RapidOrder.Core.Services;

public interface IPlaceService
{
    Task<Place> CreateAsync(Place place, CancellationToken cancellationToken = default);
    Task<Place?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Place>> GetByUserAsync(long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Place>> GetBySetupAsync(int setupId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Place place, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
