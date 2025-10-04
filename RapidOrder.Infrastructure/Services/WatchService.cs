using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.Watches;
using RapidOrder.Core.Services;

namespace RapidOrder.Infrastructure.Services;

public class WatchService : IWatchService
{
    private readonly RapidOrderDbContext _dbContext;
    private readonly IEmployeeService _employeeService;

    public WatchService(RapidOrderDbContext dbContext, IEmployeeService employeeService)
    {
        _dbContext = dbContext;
        _employeeService = employeeService;
    }

    public async Task<PagedResult<WatchSummary>> GetAllWatchesAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var normalized = pagination.Normalize();

        var query = _dbContext.Watches.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(w => w.Serial)
            .Skip((normalized.PageNumber - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .Select(w => new WatchSummary(
                w.Id,
                w.Serial,
                w.AssignedUserId,
                w.AssignedUser != null ? w.AssignedUser.DisplayName : null,
                w.BatteryPercent,
                w.LastSeenAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<WatchSummary>(items, totalCount, normalized.PageNumber, normalized.PageSize);
    }

    public async Task AssignWatchAsync(int watchId, long employeeId, CancellationToken cancellationToken = default)
    {
        var watch = await _dbContext.Watches.FirstOrDefaultAsync(w => w.Id == watchId, cancellationToken);
        if (watch == null)
        {
            throw new KeyNotFoundException($"Watch {watchId} not found");
        }

        if (!await _employeeService.EmployeeExistsAsync(employeeId, cancellationToken))
        {
            throw new KeyNotFoundException($"Employee {employeeId} not found");
        }

        watch.AssignedUserId = employeeId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UnassignWatchAsync(int watchId, CancellationToken cancellationToken = default)
    {
        var watch = await _dbContext.Watches.FirstOrDefaultAsync(w => w.Id == watchId, cancellationToken);
        if (watch == null)
        {
            throw new KeyNotFoundException($"Watch {watchId} not found");
        }

        watch.AssignedUserId = null;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> WatchExistsAsync(int watchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Watches
            .AsNoTracking()
            .AnyAsync(w => w.Id == watchId, cancellationToken);
    }
}
