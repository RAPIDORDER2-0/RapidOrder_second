using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RapidOrder.Core.Entities
{
    public abstract class AbstractAuditingEntity
    {
        [Required]
        [MaxLength(50)]
        [JsonIgnore]
        public string CreatedBy { get; set; } = string.Empty;

        [JsonIgnore]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        [JsonIgnore]
        public string? LastModifiedBy { get; set; }

        [JsonIgnore]
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
