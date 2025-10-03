using System.ComponentModel.DataAnnotations;

namespace RapidOrder.Core.Entities
{
    public class Authority
    {
        [Key]
        [MaxLength(50)]
        [Required]
        public string Name { get; set; } = string.Empty;

        public override bool Equals(object? obj)
            => obj is Authority other && string.Equals(Name, other.Name, StringComparison.Ordinal);

        public override int GetHashCode()
            => Name.GetHashCode(StringComparison.Ordinal);

        public override string ToString()
            => $"Authority{{name='{Name}'}}";
    }
}

