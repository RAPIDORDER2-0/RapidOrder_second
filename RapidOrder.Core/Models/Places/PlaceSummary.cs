namespace RapidOrder.Core.Models.Places;

public sealed record PlaceSummary(
    int Id,
    int Number,
    string Description,
    int? PlaceGroupId,
    string? PlaceGroupName,
    long? AssignedUserId);
