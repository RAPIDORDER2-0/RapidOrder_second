namespace RapidOrder.Core.Models.PlaceGroups;

public sealed record PlaceGroupSummary(
    int Id,
    string Name,
    int? Number,
    string? Description,
    long? AssignedUserId,
    int PlaceCount);
