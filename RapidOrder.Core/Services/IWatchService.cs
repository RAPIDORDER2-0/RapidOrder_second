using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.Watches;

namespace RapidOrder.Core.Services;

public interface IWatchService
{
    Task<PagedResult<WatchSummary>> GetAllWatchesAsync(PaginationParameters pagination, CancellationToken cancellationToken = default);
    Task AssignWatchAsync(int watchId, long employeeId, CancellationToken cancellationToken = default);
    Task UnassignWatchAsync(int watchId, CancellationToken cancellationToken = default);
    Task<bool> WatchExistsAsync(int watchId, CancellationToken cancellationToken = default);
}
