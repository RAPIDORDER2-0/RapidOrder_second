using System.ComponentModel.DataAnnotations;

namespace RapidOrder.Core.Entities;

public class Setting : AbstractAuditingEntity
{
    public long Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Value { get; set; }
}
