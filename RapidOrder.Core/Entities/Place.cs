using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RapidOrder.Core.Entities
{
    public class Place : AbstractAuditingEntity
    {
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }

        [StringLength(128)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Icon { get; set; }

        public int? PlaceGroupId { get; set; }

        [JsonIgnore]
        public PlaceGroup? PlaceGroup { get; set; }

        public int? SetupId { get; set; }

        [JsonIgnore]
        public Setup? Setup { get; set; }

        public long? UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        [JsonIgnore]
        public ICollection<Mission> Missions { get; set; } = new List<Mission>();

        [JsonIgnore]
        public ICollection<CallButton> CallButtons { get; set; } = new List<CallButton>();
    }
}
