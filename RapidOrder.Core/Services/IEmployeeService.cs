using RapidOrder.Core.Models.Employees;
using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.Places;

namespace RapidOrder.Core.Services;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeSummary>> GetAllEmployeesAsync(PaginationParameters pagination, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlaceSummary>> GetPlacesForEmployeeAsync(long employeeId, CancellationToken cancellationToken = default);
    Task SetBreakAsync(long employeeId, bool isOnBreak, CancellationToken cancellationToken = default);
    Task<bool> EmployeeExistsAsync(long employeeId, CancellationToken cancellationToken = default);
}
