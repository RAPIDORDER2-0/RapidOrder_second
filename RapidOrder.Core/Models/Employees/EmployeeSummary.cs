using RapidOrder.Core.Models.Places;

namespace RapidOrder.Core.Models.Employees;

public sealed record EmployeeSummary(
    long Id,
    string DisplayName,
    bool IsOnBreak,
    DateTime? BreakStartedAt,
    IReadOnlyList<PlaceSummary> Places);
