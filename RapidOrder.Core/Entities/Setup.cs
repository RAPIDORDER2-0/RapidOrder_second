using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RapidOrder.Core.Entities
{
    public class Setup : AbstractAuditingEntity
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Icon { get; set; }

        [JsonIgnore]
        public ICollection<PlaceGroup> PlaceGroups { get; set; } = new List<PlaceGroup>();

        [JsonIgnore]
        public ICollection<Place> Places { get; set; } = new List<Place>();
    }
}
