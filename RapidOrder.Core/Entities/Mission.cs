using RapidOrder.Core.Enums;

namespace RapidOrder.Core.Entities
{
    public class Mission
    {
        public long Id { get; set; }
        public MissionType Type { get; set; }
        public MissionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public long? AssignedUserId { get; set; }
        public User? AssignedUser { get; set; }

        public int? PlaceId { get; set; }
        public Place? Place { get; set; }

        public int? PlaceGroupId { get; set; }
        public PlaceGroup? PlaceGroup { get; set; }

        public int? SetupId { get; set; }
        public Setup? Setup { get; set; }

        // provenance
        public string? SourceDecoded { get; set; }
        public int? SourceButton { get; set; }

        // derived metrics (optional convenience)
        public long? MissionDurationSeconds { get; set; }
        public long? IdleTimeSeconds { get; set; }
    }
}
