using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RapidOrder.Core.Entities
{
    public class PlaceGroup : AbstractAuditingEntity
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Description { get; set; }

        public int? Number { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? SetupId { get; set; }

        [JsonIgnore]
        public Setup? Setup { get; set; }

        public long? AssignedUserId { get; set; }

        [JsonIgnore]
        public User? AssignedUser { get; set; }

        [JsonIgnore]
        public ICollection<Place> Places { get; set; } = new List<Place>();
    }
}
