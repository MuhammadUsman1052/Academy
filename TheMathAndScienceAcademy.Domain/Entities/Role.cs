using TheMathAndScienceAcademy.Domain.Common;

namespace TheMathAndScienceAcademy.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
