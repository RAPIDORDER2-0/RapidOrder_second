using Microsoft.EntityFrameworkCore;
using RapidOrder.Core.Entities;
using RapidOrder.Core.Enums;
using RapidOrder.Core.Models.Employees;
using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.Places;
using RapidOrder.Core.Services;

namespace RapidOrder.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly RapidOrderDbContext _dbContext;

    public EmployeeService(RapidOrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<EmployeeSummary>> GetAllEmployeesAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var normalized = pagination.Normalize();

        var baseQuery = _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.WAITER || u.Role == UserRole.MANAGER);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var employees = await baseQuery
            .OrderBy(u => u.DisplayName)
            .Skip((normalized.PageNumber - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .Select(u => new EmployeeSummary(
                u.Id,
                u.DisplayName,
                u.IsOnBreak,
                u.BreakStartedAt,
                _dbContext.Places
                    .Where(p => p.UserId == u.Id)
                    .OrderBy(p => p.Number)
                    .Select(p => new PlaceSummary(
                        p.Id,
                        p.Number,
                        p.Description,
                        p.PlaceGroupId,
                        p.PlaceGroup != null ? p.PlaceGroup.Name : null,
                        p.UserId))
                    .ToList()))
            .ToListAsync(cancellationToken);

        return new PagedResult<EmployeeSummary>(employees, totalCount, normalized.PageNumber, normalized.PageSize);
    }

    public async Task<IReadOnlyList<PlaceSummary>> GetPlacesForEmployeeAsync(long employeeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Places
            .AsNoTracking()
            .Where(p => p.UserId == employeeId)
            .OrderBy(p => p.Number)
            .Select(p => new PlaceSummary(
                p.Id,
                p.Number,
                p.Description,
                p.PlaceGroupId,
                p.PlaceGroup != null ? p.PlaceGroup.Name : null,
                p.UserId))
            .ToListAsync(cancellationToken);
    }

    public async Task SetBreakAsync(long employeeId, bool isOnBreak, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == employeeId, cancellationToken);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee {employeeId} not found");
        }

        if (isOnBreak && employee.IsOnBreak)
        {
            throw new InvalidOperationException("Employee is already on break.");
        }

        employee.IsOnBreak = isOnBreak;
        employee.BreakStartedAt = isOnBreak ? DateTime.UtcNow : null;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == employeeId, cancellationToken);
    }
}
