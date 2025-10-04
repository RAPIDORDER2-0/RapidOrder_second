using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.PlaceGroups;
using RapidOrder.Core.Models.Places;
using RapidOrder.Core.Services;

namespace RapidOrder.Infrastructure.Services;

public class PlaceAssignmentService : IPlaceAssignmentService
{
    private readonly RapidOrderDbContext _dbContext;
    private readonly IEmployeeService _employeeService;

    public PlaceAssignmentService(RapidOrderDbContext dbContext, IEmployeeService employeeService)
    {
        _dbContext = dbContext;
        _employeeService = employeeService;
    }

    public async Task<PagedResult<PlaceSummary>> GetPlacesAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var normalized = pagination.Normalize();

        var query = _dbContext.Places.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Number)
            .Skip((normalized.PageNumber - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .Select(p => new PlaceSummary(
                p.Id,
                p.Number,
                p.Description,
                p.PlaceGroupId,
                p.PlaceGroup != null ? p.PlaceGroup.Name : null,
                p.UserId))
            .ToListAsync(cancellationToken);

        return new PagedResult<PlaceSummary>(items, totalCount, normalized.PageNumber, normalized.PageSize);
    }

    public async Task<IReadOnlyList<PlaceGroupSummary>> GetPlaceGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.PlaceGroups
            .AsNoTracking()
            .Include(pg => pg.Places)
            .OrderBy(pg => pg.Number)
            .ThenBy(pg => pg.Name)
            .Select(pg => new PlaceGroupSummary(
                pg.Id,
                pg.Name,
                pg.Number,
                pg.Description,
                pg.AssignedUserId,
                pg.Places.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task AssignPlaceToEmployeeAsync(int placeId, long employeeId, CancellationToken cancellationToken = default)
    {
        var place = await _dbContext.Places.FirstOrDefaultAsync(p => p.Id == placeId, cancellationToken);
        if (place == null)
        {
            throw new KeyNotFoundException($"Place {placeId} not found");
        }

        if (!await _employeeService.EmployeeExistsAsync(employeeId, cancellationToken))
        {
            throw new KeyNotFoundException($"Employee {employeeId} not found");
        }

        place.UserId = employeeId;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignPlaceGroupToEmployeeAsync(int placeGroupId, long employeeId, CancellationToken cancellationToken = default)
    {
        var group = await _dbContext.PlaceGroups
            .Include(pg => pg.Places)
            .FirstOrDefaultAsync(pg => pg.Id == placeGroupId, cancellationToken);

        if (group == null)
        {
            throw new KeyNotFoundException($"Place group {placeGroupId} not found");
        }

        if (!await _employeeService.EmployeeExistsAsync(employeeId, cancellationToken))
        {
            throw new KeyNotFoundException($"Employee {employeeId} not found");
        }

        group.AssignedUserId = employeeId;

        foreach (var place in group.Places)
        {
            place.UserId = employeeId;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> PlaceExistsAsync(int placeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Places
            .AsNoTracking()
            .AnyAsync(p => p.Id == placeId, cancellationToken);
    }

    public async Task<bool> PlaceGroupExistsAsync(int placeGroupId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PlaceGroups
            .AsNoTracking()
            .AnyAsync(pg => pg.Id == placeGroupId, cancellationToken);
    }
}
