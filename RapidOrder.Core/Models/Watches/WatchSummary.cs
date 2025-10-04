namespace RapidOrder.Core.Models.Watches;

public sealed record WatchSummary(
    int Id,
    string Serial,
    long? AssignedUserId,
    string? AssignedUserName,
    int BatteryPercent,
    DateTime? LastSeenAt);
