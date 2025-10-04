using RapidOrder.Core.Models.Pagination;
using RapidOrder.Core.Models.PlaceGroups;
using RapidOrder.Core.Models.Places;

namespace RapidOrder.Core.Services;

public interface IPlaceAssignmentService
{
    Task<PagedResult<PlaceSummary>> GetPlacesAsync(PaginationParameters pagination, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlaceGroupSummary>> GetPlaceGroupsAsync(CancellationToken cancellationToken = default);
    Task AssignPlaceToEmployeeAsync(int placeId, long employeeId, CancellationToken cancellationToken = default);
    Task AssignPlaceGroupToEmployeeAsync(int placeGroupId, long employeeId, CancellationToken cancellationToken = default);
    Task<bool> PlaceExistsAsync(int placeId, CancellationToken cancellationToken = default);
    Task<bool> PlaceGroupExistsAsync(int placeGroupId, CancellationToken cancellationToken = default);
}
