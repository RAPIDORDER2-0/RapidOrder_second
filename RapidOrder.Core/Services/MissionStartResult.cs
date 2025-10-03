using RapidOrder.Core.Entities;

namespace RapidOrder.Core.Services;

public sealed record MissionStartResult(Mission Mission, bool CreatedNew);
